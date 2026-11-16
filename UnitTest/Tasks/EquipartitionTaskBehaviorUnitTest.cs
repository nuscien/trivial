using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Web;

using States = Trivial.Tasks.EquipartitionTask.FragmentStates;

namespace Trivial.Tasks;

[TestClass]
public class EquipartitionTaskBehaviorUnitTest
{
    [TestMethod]
    public void PickRespectsOrderExclusionsAndRetryOptions()
    {
        var task = new EquipartitionTask("job", 3);
        var first = task.Pick("worker-a");
        Assert.AreSame(task[0], first);
        Assert.AreEqual(States.Working, first.State);
        Assert.AreEqual("worker-a", first.Tag);
        Assert.IsTrue(task.UpdateFragment(first, States.Failure));

        var second = task.Pick(except: new[] { first.Id });
        Assert.AreSame(task[1], second);
        Assert.AreEqual(States.Working, second.State);
        var third = task.Pick(onlyPending: true);
        Assert.AreSame(task[2], third);
        Assert.IsNull(task.Pick(onlyPending: true));

        var retry = task.Pick("worker-b");
        Assert.AreSame(first, retry);
        Assert.AreEqual(States.Retrying, retry.State);
        Assert.AreEqual("worker-b", retry.Tag);
        Assert.IsNull(task.Pick());
    }

    [TestMethod]
    public void ExactStatePickCanSelectTerminalFragmentWithoutChangingState()
    {
        var task = new EquipartitionTask("job", 1);
        Assert.IsTrue(task.UpdateFragment(task[0], States.Success));
        var events = 0;
        task.FragmentStateChanged += (_, e) =>
        {
            events++;
            Assert.AreEqual(States.Success, e.OldValue);
            Assert.AreEqual(States.Success, e.NewValue);
        };

        Assert.AreSame(task[0], task.Pick("reviewed", States.Success));
        Assert.AreEqual(States.Success, task[0].State);
        Assert.AreEqual("reviewed", task[0].Tag);
        Assert.AreEqual(1, events);
        Assert.IsNull(task.Pick(States.Failure));
    }

    [TestMethod]
    public void UpdateEnforcesTerminalAndPendingRulesAndNormalizesProcessingStates()
    {
        var task = new EquipartitionTask("job", 1);
        var fragment = task[0];
        Assert.IsFalse(task.UpdateFragment("missing", States.Success));
        Assert.IsTrue(task.UpdateFragment(fragment, States.Retrying));
        Assert.AreEqual(States.Working, fragment.State);
        Assert.IsTrue(task.UpdateFragment(fragment, States.Pending));
        Assert.AreEqual(States.Pending, fragment.State);
        Assert.IsTrue(task.UpdateFragment(fragment, States.Failure));
        Assert.IsFalse(task.UpdateFragment(fragment, States.Pending));
        Assert.AreEqual(States.Failure, fragment.State);
        Assert.IsTrue(task.UpdateFragment(fragment, States.Working));
        Assert.AreEqual(States.Retrying, fragment.State);
        Assert.IsTrue(task.UpdateFragment(fragment, States.Fatal));
        Assert.IsFalse(task.UpdateFragment(fragment, States.Success));
        Assert.AreEqual(States.Fatal, fragment.State);
        Assert.IsTrue(task.UpdateFragment(fragment, States.Fatal));
    }

    [TestMethod]
    public void TagOnlyAndCopyUpdatesNotifyAndSynchronize()
    {
        var task = new EquipartitionTask("job", 1);
        var fragment = task.Pick("initial");
        var copy = EquipartitionTask.Fragment.Parse(fragment.ToJsonString());
        var changes = new List<(States Old, States New, string Tag)>();
        task.FragmentStateChanged += (_, e) => changes.Add((e.OldValue, e.NewValue, e.Source.Tag));
        Assert.IsTrue(task.UpdateFragment(copy, null, "replacement"));
        Assert.AreEqual("replacement", fragment.Tag);
        Assert.AreEqual("replacement", copy.Tag);
        Assert.AreEqual(States.Working, copy.State);
        Assert.IsTrue(task.UpdateFragment(copy, States.Success));
        Assert.AreEqual(States.Success, copy.State);
        Assert.AreEqual(fragment.Modification, copy.Modification);
        Assert.IsTrue(task.UpdateFragment(fragment.Id, null, null));
        Assert.IsNull(fragment.Tag);
        Assert.AreEqual(3, changes.Count);
        Assert.AreEqual((States.Working, States.Working, "replacement"), changes[0]);
        Assert.AreEqual((States.Working, States.Success, "replacement"), changes[1]);
        Assert.AreEqual((States.Success, States.Success, (string)null), changes[2]);
    }

    [TestMethod]
    public void CompletionAndDescriptionEventsReflectAcceptedOperations()
    {
        var task = new EquipartitionTask("job", 2);
        var notifications = new List<string>();
        task.DescriptionChanged += (_, e) => notifications.Add($"description:{e.OldValue}->{e.NewValue}");
        task.FragmentStateChanged += (_, e) => notifications.Add($"{e.Source.Index}:{e.OldValue}->{e.NewValue}");
        task.HasBeenDone += (_, _) => notifications.Add("done");
        task.Description = "batch";
        task.Description = "batch";
        Assert.IsTrue(task.UpdateFragment(task[0], States.Success));
        Assert.IsFalse(task.IsDone);
        Assert.IsTrue(task.UpdateFragment(task[1], States.Fatal));
        Assert.IsTrue(task.IsDone);
        Assert.IsTrue(task.UpdateFragment(task[1], States.Fatal));
        Assert.IsFalse(task.UpdateFragment(task[1], States.Success));
        CollectionAssert.AreEqual(new[]
        {
            "description:->batch", "description:batch->batch", "0:Pending->Success",
            "1:Pending->Fatal", "done", "1:Fatal->Fatal"
        }, notifications);
    }

    [TestMethod]
    public void CancelNotifiesRemainingFragmentsBeforeCompletionAndIsIdempotent()
    {
        var task = new EquipartitionTask("job", 3);
        Assert.IsTrue(task.UpdateFragment(task[0], States.Success));
        Assert.IsTrue(task.UpdateFragment(task[1], States.Failure));
        var notifications = new List<string>();
        task.FragmentStateChanged += (_, e) => notifications.Add($"{e.Source.Index}:{e.OldValue}->{e.NewValue}");
        task.HasBeenDone += (_, _) => notifications.Add("done");
        task.Cancel();
        task.Cancel();

        CollectionAssert.AreEqual(new[] { "1:Failure->Ignored", "2:Pending->Ignored", "done" }, notifications);
        Assert.AreEqual(States.Success, task[0].State);
        Assert.AreEqual(States.Ignored, task[1].State);
        Assert.AreEqual(States.Ignored, task[2].State);
        Assert.IsTrue(task.IsDone);
    }

    [TestMethod]
    public void FiltersClassifyAllStatesIncludingNonterminalFailure()
    {
        var states = new[] { States.Pending, States.Working, States.Success, States.Failure, States.Retrying, States.Fatal, States.Ignored };
        var fragments = states.Select((state, index) => new EquipartitionTask.Fragment(index.ToString(), index, state, null, null)).ToArray();
        var task = new EquipartitionTask("task", "job", fragments);
        CollectionAssert.AreEqual(new[] { 0, 3 }, task.GetWaitingFragments().Select(f => f.Index).ToArray());
        CollectionAssert.AreEqual(new[] { 1, 4 }, task.GetProcessingFragments().Select(f => f.Index).ToArray());
        CollectionAssert.AreEqual(new[] { 2, 5, 6 }, task.GetDoneFragments().Select(f => f.Index).ToArray());
        CollectionAssert.AreEqual(new[] { 3, 5 }, task.GetErrorFragments().Select(f => f.Index).ToArray());
        CollectionAssert.AreEqual(new[] { 3, 5, 6 }, task.GetErrorOrIngoredFragments().Select(f => f.Index).ToArray());
        CollectionAssert.AreEqual(new[] { 2, 6 }, task.Where(States.Success, States.Ignored).Select(f => f.Index).ToArray());
        Assert.IsFalse(task.IsDone);
    }

    [TestMethod]
    public void RestorePreservesFragmentsAndHandlesEmptyTask()
    {
        var creation = new DateTime(2020, 1, 2);
        var first = new EquipartitionTask.Fragment("first", 4, States.Failure, creation, creation, "tag");
        var duplicateIndex = new EquipartitionTask.Fragment("second", 4, States.Success, creation, creation);
        var blank = new EquipartitionTask.Fragment(" ", 0, States.Pending, null, null);
        var task = new EquipartitionTask("task-id", "job-id", new[] { first, null, blank, duplicateIndex }, creation, "restored");
        Assert.AreEqual("task-id", task.Id);
        Assert.AreEqual("job-id", task.JobId);
        Assert.AreEqual(creation, task.Creation);
        Assert.AreEqual("restored", task.Description);
        Assert.AreEqual(2, task.Count);
        Assert.AreEqual(2, task.GetByFragmentIndex(4).Count());
        Assert.AreSame(first, task["first"]);
        Assert.IsFalse(task.TryGetByFragmentId("missing", out var missing));
        Assert.IsNull(missing);
        var empty = new EquipartitionTask("job", 0);
        Assert.IsTrue(empty.IsDone);
        Assert.IsNull(empty.Pick());
        Assert.AreEqual(empty.Creation, empty.Modification);
    }

    [TestMethod]
    public void TaskJsonIncludesExpectedFieldsAndEscapedValues()
    {
        var creation = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var modification = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc);
        var fragment = new EquipartitionTask.Fragment("fragment-id", 3, States.Failure, creation, modification, "worker");
        var task = new EquipartitionTask("task-id", "job\"id", new[] { fragment }, creation, "line\nquoted\"");
        using var document = JsonDocument.Parse(task.ToJsonString());
        var root = document.RootElement;
        Assert.AreEqual("task-id", root.GetProperty("id").GetString());
        Assert.AreEqual("job\"id", root.GetProperty("job").GetString());
        Assert.AreEqual(WebFormat.ParseDate(creation).ToString(CultureInfo.InvariantCulture), root.GetProperty("creation").GetString());
        Assert.AreEqual(WebFormat.ParseDate(modification).ToString(CultureInfo.InvariantCulture), root.GetProperty("update").GetString());
        Assert.AreEqual(JsonValueKind.False, root.GetProperty("done").ValueKind);
        Assert.AreEqual("line\nquoted\"", root.GetProperty("desc").GetString());
        var items = root.GetProperty("fragments");
        Assert.AreEqual(1, items.GetArrayLength());
        Assert.AreEqual("fragment-id", items[0].GetProperty("id").GetString());
        Assert.AreEqual("failure", items[0].GetProperty("state").GetString());
        Assert.AreEqual(WebFormat.ParseDate(modification), items[0].GetProperty("update").GetInt64());
    }

    [TestMethod]
    public void EmptyAndOptionalTaskJsonUsesExpectedShape()
    {
        var task = new EquipartitionTask("job", 0);
        using var emptyDocument = JsonDocument.Parse(task.ToJsonString());
        var empty = emptyDocument.RootElement;
        Assert.AreEqual(JsonValueKind.True, empty.GetProperty("done").ValueKind);
        Assert.AreEqual(0, empty.GetProperty("fragments").GetArrayLength());
        Assert.AreEqual(empty.GetProperty("creation").GetString(), empty.GetProperty("update").GetString());
        Assert.IsFalse(empty.TryGetProperty("desc", out _));

        var extra = new EquipartitionTask.Fragment("additional", 9, States.Working, null, null);
        var properties = new Dictionary<string, string> { ["key\"name"] = "value\ntext", ["nullable"] = null };
        using var optionalDocument = JsonDocument.Parse(task.ToJsonString(false, extra, properties));
        var optional = optionalDocument.RootElement;
        Assert.IsFalse(optional.TryGetProperty("fragments", out _));
        Assert.AreEqual("additional", optional.GetProperty("fragment").GetProperty("id").GetString());
        Assert.AreEqual("value\ntext", optional.GetProperty("key\"name").GetString());
        Assert.AreEqual(JsonValueKind.Null, optional.GetProperty("nullable").ValueKind);
    }

    [TestMethod]
    public void FragmentSerializationRoundTripsJsonQueryAndXml()
    {
        var original = new EquipartitionTask.Fragment("fragment-id", 7, States.Retrying, null, null, "worker");
        var json = EquipartitionTask.Fragment.Parse(original.ToJsonString());
        var query = EquipartitionTask.Fragment.Parse(original.ToQueryData().ToString());
        var xml = EquipartitionTask.Fragment.Parse("<fragment><id>fragment-id</id><index>7</index><state>retrying</state><tag>worker</tag></fragment>");
        foreach (var restored in new[] { json, query, xml })
        {
            Assert.AreEqual(original.Id, restored.Id);
            Assert.AreEqual(original.Index, restored.Index);
            Assert.AreEqual(original.State, restored.State);
            Assert.AreEqual(original.Tag, restored.Tag);
        }

        var fallback = EquipartitionTask.Fragment.Parse("id=x&index=not-a-number&state=unknown");
        Assert.AreEqual(0, fallback.Index);
        Assert.AreEqual(States.Pending, fallback.State);
    }

    [TestMethod]
    public void ContainerGroupsRetainsOrRemovesCompletedTasks()
    {
        var container = new EquipartitionTaskContainer();
        var created = 0;
        container.Created += (_, _) => created++;
        var retained = container.Create("group", "first", 1, autoRemove: false);
        var removed = container.Create("group", "second", 1);
        var other = container.Create("other", "third", 1);
        Assert.AreEqual(3, created);
        var selected = container.Pick("group", "worker");
        Assert.IsTrue(selected.IsSelected);
        Assert.AreSame(retained, selected.Parent);
        Assert.AreEqual("worker", selected.ItemSelected.Tag);
        Assert.IsTrue(retained.UpdateFragment(selected.ItemSelected, States.Success));
        Assert.AreEqual(1, container.List("group").Count());
        Assert.AreEqual(2, container["group"].Count);
        Assert.IsTrue(removed.UpdateFragment(removed[0], States.Fatal));
        Assert.AreEqual(0, container.List("group").Count());
        Assert.AreEqual(1, container["group"].Count);
        Assert.AreSame(other, container.TryGetFirst("other"));
        Assert.IsFalse(container.Pick("group").IsSelected);
    }

    [TestMethod]
    public void ConcurrentPicksAssignEachPendingFragmentAtMostOnce()
    {
        var task = new EquipartitionTask("job", 100);
        var picked = new ConcurrentBag<string>();
        Parallel.For(0, 200, _ =>
        {
            var fragment = task.Pick();
            if (fragment != null) picked.Add(fragment.Id);
        });
        Assert.AreEqual(100, picked.Count);
        Assert.AreEqual(100, picked.Distinct().Count());
        Assert.AreEqual(100, task.GetProcessingFragments().Count());
    }
}
