# Equipartition Task

In the `Trivial.Tasks` [namespace](../).

<!-- Start -->

`EquipartitionTask` tracks a job as a fixed list of fragments that workers can pick and update. It tracks identifiers, indexes, states, tags, and timestamps; it does **not** split data, execute queries, retry work automatically, or communicate with workers. The application must define a stable partition rule and arrange execution and persistence.

## Creating and processing fragments

```csharp
using Trivial.Tasks;
using States = Trivial.Tasks.EquipartitionTask.FragmentStates;

var task = new EquipartitionTask("customer-reindex", 5);
var fragment = task.Pick("worker-1");
if (fragment != null)
{
    // Example rule for nonnegative IDs: process rows where ID % task.Count == fragment.Index.
    // Execute that query and the work in the application, then report its actual outcome.
    bool succeeded = ProcessPartition(fragment.Index, task.Count);
    task.UpdateFragment(fragment, succeeded ? States.Success : States.Failure);
}
```

`ProcessPartition` represents application code. Choose a rule appropriate to the data, with disjoint and complete partitions, and keep the fragment count and rule stable across retries. The task creates indexes `0` through `count - 1`; non-positive counts create no fragments. An empty task is already done, and its `Modification` property returns its creation time.

`Pick()` selects the first pending or failed fragment in list order, changing it to `Working` or `Retrying` respectively; it returns null if none is eligible, even if other fragments are still working. An earlier failed fragment can therefore be selected before a later pending one. Use `onlyPending: true` to exclude failures, `except` to exclude IDs, or a tag to record a worker or other metadata. The `Pick(state)` overload matches that exact state: pending and failure are started as above, while other states are returned unchanged. Picking and updating report fragment changes through `FragmentStateChanged`.

## States and updates

| State | Meaning | Classification |
| --- | --- | --- |
| `Pending` | Waiting for its first assignment | Waiting |
| `Working` | Initial attempt in progress | Processing |
| `Failure` | Attempt failed and may be picked again | Waiting, error |
| `Retrying` | Subsequent attempt in progress | Processing |
| `Success` | Completed successfully | Done |
| `Fatal` | Permanently failed | Done, error |
| `Ignored` | Canceled or intentionally skipped | Done |

A typical path is `Pending -> Working -> Failure -> Retrying -> Success`, with `Fatal` or `Ignored` as alternative terminal outcomes. `UpdateFragment` also permits other direct transitions between nonterminal and terminal states. A transition back to `Pending` is allowed only from `Working`. When the requested state differs from the current state, a request for `Working` or `Retrying` becomes `Working` from `Pending`, otherwise `Retrying` for a nonterminal fragment. Terminal states cannot change to another state, but an update to the same state is accepted. Check the returned Boolean: an unknown ID or rejected transition returns false. Tag overloads replace the tag, including clearing it with null; a null state changes only the tag. A separately deserialized fragment with the same ID is synchronized when its update succeeds.

`IsDone` means every fragment is terminal, not that every fragment succeeded. The `GetWaitingFragments`, `GetProcessingFragments`, `GetDoneFragments`, `GetErrorFragments`, `GetErrorOrIngoredFragments`, and `Where` methods filter the live fragments. `Cancel()` changes all nonterminal fragments to `Ignored`; it does not stop work already running externally.

## Notifications and restoration

`FragmentStateChanged` is raised synchronously after a pick or accepted update, including a same-state or tag-only update. `HasBeenDone` is raised when an operation completes the task. Cancellation notifies each changed fragment and then signals completion; repeated cancellation of an already completed task does nothing. `DescriptionChanged` is raised whenever the description setter is called. Event handlers are application code, not durable delivery; persist and handle errors according to your requirements.

`task.ToJsonString()` serializes the task and its fragments, including an empty fragment array for an empty task. The task's `creation` and `update` values are strings containing Unix milliseconds; `done` is a JSON Boolean. Fragment timestamps are JSON numbers. The overload can omit the fragment array and add a separate `fragment` or extra string properties; avoid extra property names that collide with the built-in fields. There is no task-level parse method: when restoring a task, read its saved ID, job ID, fragments, creation time, and description and pass them to the corresponding constructor. Null fragments and fragments with blank IDs are omitted; duplicate indexes are retained.

A fragment can also be serialized with `ToJsonString()` or `ToQueryData()` and restored with `Fragment.Parse` (JSON, query string, or XML).

## Grouped tasks

```csharp
var container = new EquipartitionTaskContainer();
var task1 = container.Create("service-1", "job-1", 5, autoRemove: false);
var task2 = container.Create("service-1", "job-2", 1);
var selection = container.Pick("service-1", "worker-1");
if (selection.IsSelected)
{
    var selectedTask = selection.Parent;
    var selectedFragment = selection.ItemSelected;
    // Process and report the outcome using selectedTask.UpdateFragment(...).
}
```

`List(group)` returns unfinished tasks only; the group indexer also exposes completed tasks that remain in the container. By default a newly created task is removed when it raises `HasBeenDone`; use `autoRemove: false` to retain it. `Created` is raised by the overload that creates a task from a job ID and count. Container and collection `UpdateFragment` return a selection indicating whether the fragment was found, not whether the requested transition succeeded; use the task's Boolean-returning method when that distinction matters.

## Concurrency and distribution

Picks use an instance lock for selection and state changes. State-changing updates also use that lock, but read the old state before acquiring it, so competing updates are not guaranteed to validate against the latest state. Reads, tag-only updates, event handlers, and container access are not an atomic transaction either. Coordinate competing operations externally. Separate processes or independently restored instances do not share that lock: route assignments through a shared coordinator or implement atomic persistence/leases, worker failure recovery, and idempotent processing yourself. The in-memory container is not a distributed queue or durable store.
