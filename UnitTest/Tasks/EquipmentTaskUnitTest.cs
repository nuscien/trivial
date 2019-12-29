using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Tasks;

namespace Trivial.UnitTest.Tasks
{
    [TestClass]
    public class EquipmentTaskUnitTest
    {
        [TestMethod]
        public async Task TestTaskAsync()
        {
            var tasks = new EquipartitionTaskContainer();
            var task1 = tasks.Create("a", "o", 5, "task1");
            var task2 = tasks.Create("a", "p", 1, "task2");
            var task3 = tasks.Create("b", "q", 2, "task3");

            Assert.AreEqual(5, task1.Count);
            Assert.AreEqual(1, task2.Count);
            Assert.AreEqual(2, task3.Count);
            Assert.AreEqual(5, task1.GetWaitingOrProcessingFragments().Count());
            Assert.AreEqual(1, task2.GetWaitingOrProcessingFragments().Count());
            Assert.AreEqual(2, task3.GetWaitingOrProcessingFragments().Count());
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            Assert.AreEqual(0, task2.GetProcessingFragments().Count());
            Assert.AreEqual(0, task3.GetProcessingFragments().Count());
            Assert.AreEqual(0, task1.GetDoneFragments().Count());
            Assert.AreEqual(0, task2.GetDoneFragments().Count());
            Assert.AreEqual(0, task3.GetDoneFragments().Count());

            var f = task1.Pick();
            Assert.AreEqual(EquipartitionTask.FragmentStates.Working, f.State);
            Assert.AreEqual(1, task1.GetProcessingFragments().Count());
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            Assert.AreEqual(EquipartitionTask.FragmentStates.Success, f.State);
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            f = task1.Pick();
            Assert.AreEqual(EquipartitionTask.FragmentStates.Working, f.State);
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Failure);
            Assert.AreEqual(EquipartitionTask.FragmentStates.Failure, f.State);
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            f = task1.Pick();
            Assert.AreEqual(EquipartitionTask.FragmentStates.Retrying, f.State);
            Assert.AreEqual(1, task1.GetProcessingFragments().Count());
            Assert.AreEqual(0, task2.GetProcessingFragments().Count());
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Fatal);
            Assert.AreEqual(EquipartitionTask.FragmentStates.Fatal, f.State);

            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            Assert.AreEqual(0, task2.GetProcessingFragments().Count());
            Assert.AreEqual(0, task3.GetProcessingFragments().Count());
            Assert.AreEqual(2, task1.GetDoneFragments().Count());
            Assert.AreEqual(0, task2.GetDoneFragments().Count());
            Assert.AreEqual(0, task3.GetDoneFragments().Count());
            Assert.AreEqual(3, task1.GetWaitingOrProcessingFragments().Count());

            task3.Pick();
            f = task3.Pick();
            Assert.AreEqual(2, task3.GetProcessingFragments().Count());
            var fStr = f.ToJsonString();
            f = EquipartitionTask.Fragment.Parse(fStr);
            fStr = f.ToQueryData().ToString();
            f = EquipartitionTask.Fragment.Parse(fStr);
            var status = task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            Assert.AreEqual(2, task3.GetProcessingFragments().Count());
            Assert.AreEqual(false, status);
            task3.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
            Assert.AreEqual(1, task3.GetProcessingFragments().Count());
            Assert.AreEqual(EquipartitionTask.FragmentStates.Success, f.State);
            f = task3.Pick();
            Assert.IsNull(f);

            task3.UpdateFragment(task3[0], EquipartitionTask.FragmentStates.Success);
            Assert.AreEqual(0, task1.GetProcessingFragments().Count());
        }
    }
}
