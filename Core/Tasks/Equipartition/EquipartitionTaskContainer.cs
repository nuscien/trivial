using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Tasks
{
    /// <summary>
    /// Equipartition task thread-safe container with grouping.
    /// </summary>
    public class EquipartitionTaskContainer
    {
        private readonly object locker = new object();
        private readonly Dictionary<string, List<EquipartitionTask>> cache = new Dictionary<string, List<EquipartitionTask>>();

        /// <summary>
        /// Adds or removes an event handler when create a new task.
        /// </summary>
        public event ChangeEventHandler<EquipartitionTask> Created;

        /// <summary>
        /// Gets the equipartition task list.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <returns>A list of the equipartition task.</returns>
        public IReadOnlyList<EquipartitionTask> this[string group] => (cache.TryGetValue(group, out var list) ? list : new List<EquipartitionTask>()).AsReadOnly();

        /// <summary>
        /// Gets the equipartition task list.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <returns>A list of the equipartition task.</returns>
        public EquipartitionTask this[string group, string taskId] => this[group].First(t => t.Id == taskId);

        /// <summary>
        /// Gets the group identifier list in cache.
        /// </summary>
        public IReadOnlyCollection<string> GroupIds => cache.Keys;

        /// <summary>
        /// Gets all the equipartition tasks available.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="count">The count to take.</param>
        /// <returns>A collection of equipartition task.</returns>
        public IEnumerable<EquipartitionTask> List(string group, int? count = null)
        {
            return List(group, null, count);
        }

        /// <summary>
        /// Gets all the equipartition tasks available.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="count">The count to take.</param>
        /// <returns>A collection of equipartition task.</returns>
        public IEnumerable<EquipartitionTask> List(string group, Func<EquipartitionTask, bool> predicate, int? count = null)
        {
            if (!cache.TryGetValue(group, out var list)) return new List<EquipartitionTask>().AsReadOnly();
            var col = list.Where(ele =>
            {
                return !ele.IsDone;
            });
            if (predicate != null) col = col.Where(predicate);
            return count.HasValue ? col.Take(count.Value) : col;
        }

        /// <summary>
        /// Gets the first equipartition tasks if available.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An equipartition task; or null, if no one available.</returns>
        public EquipartitionTask TryGetFirst(string group, Func<EquipartitionTask, bool> predicate = null)
        {
            return List(group, predicate).FirstOrDefault();
        }

        /// <summary>
        /// Gets the last equipartition tasks if available.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An equipartition task; or null, if no one available.</returns>
        public EquipartitionTask TryGetLast(string group, Func<EquipartitionTask, bool> predicate = null)
        {
            return List(group, predicate).LastOrDefault();
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="pick">A handler to pick.</param>
        /// <returns>A task and fragment.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(string group, Func<EquipartitionTask, EquipartitionTask.Fragment> pick = null)
        {
            return Pick(group, null, pick);
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A task and fragment.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(string group, string tag, IEnumerable<string> except = null, bool onlyPending = false)
        {
            return Pick(group, null, task => task.Pick(tag, except, onlyPending));
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pick">A handler to pick.</param>
        /// <returns>A task and fragment.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(string group, Func<EquipartitionTask, bool> predicate, Func<EquipartitionTask, EquipartitionTask.Fragment> pick = null)
        {
            var col = List(group, predicate);
            if (pick == null) pick = task => task.Pick();
            foreach (var item in col)
            {
                var f = pick(item);
                if (f == null) continue;
                return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>(item, f);
            }

            return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>();
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A task and fragment.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(string group, Func<EquipartitionTask, bool> predicate, string tag, IEnumerable<string> except = null, bool onlyPending = false)
        {
            return Pick(group, predicate, task => task.Pick(tag, except, onlyPending));
        }

        /// <summary>
        /// Creates a new equipartition tasks.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="count">The task fragment count.</param>
        /// <param name="autoRemove">true if remove the item all done automatically; otherwise, false.</param>
        /// <returns>A new equipartition tasks created.</returns>
        public EquipartitionTask Create(string group, string jobId, int count, bool autoRemove = true)
        {
            return Create(group, jobId, count, null, autoRemove);
        }

        /// <summary>
        /// Creates a new equipartition tasks.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="count">The task fragment count.</param>
        /// <param name="description">The description.</param>
        /// <param name="autoRemove">true if remove the item all done automatically; otherwise, false.</param>
        /// <returns>A new equipartition tasks created.</returns>
        public virtual EquipartitionTask Create(string group, string jobId, int count, string description, bool autoRemove = true)
        {
            var task = Create(group, new EquipartitionTask(jobId, count)
            {
                Description = description
            }, autoRemove);
            Created?.Invoke(this, new ChangeEventArgs<EquipartitionTask>(null, task, ChangeMethods.Add, group));
            return task;
        }

        /// <summary>
        /// Creates a new equipartition tasks.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="task">The task to create.</param>
        /// <param name="autoRemove">true if remove the item all done automatically; otherwise, false.</param>
        /// <returns>A new equipartition tasks created.</returns>
        public EquipartitionTask Create(string group, EquipartitionTask task, bool autoRemove = true)
        {
            return Create(group, new[] { task }, autoRemove) > 0 ? task : null;
        }

        /// <summary>
        /// Creates a new equipartition tasks.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="tasks">The tasks to create.</param>
        /// <param name="autoRemove">true if remove the item all done automatically; otherwise, false.</param>
        /// <returns>A new equipartition tasks created.</returns>
        public virtual int Create(string group, IEnumerable<EquipartitionTask> tasks, bool autoRemove = true)
        {
            if (!cache.ContainsKey(group))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(group))
                    {
                        cache[group] = new List<EquipartitionTask>();
                    }
                }
            }

            var list = cache[group];
            var count = 0;
            tasks = tasks.Where(task =>
            {
                if (task == null) return false;
                count++;
                return true;
            });
            if (autoRemove)
            {
                foreach (var task in tasks)
                {
                    task.HasBeenDone += (object sender, EventArgs eventArgs) =>
                    {
                        list.Remove(task);
                    };
                    list.Add(task);
                }
            }
            else
            {
                list.AddRange(tasks);
            }

            return count;
        }

        /// <summary>
        /// Gets a task fragment.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> GetFragment(string group, string fragmentId)
        {
            if (!cache.TryGetValue(group, out var list)) return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>();
            foreach (var task in list)
            {
                var fragment = task.TryGetByFragmentId(fragmentId);
                if (fragment == null) continue;
                return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>(task, fragment);
            }

            return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>();
        }

        /// <summary>
        /// Updates a specific task fragment.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <param name="tag">The new tag.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> UpdateFragment(string group, string fragmentId, EquipartitionTask.FragmentStates? state, string tag)
        {
            var info = GetFragment(group, fragmentId);
            if (info.IsSelected) info.Parent.UpdateFragment(info.ItemSelected, state, tag);
            return info;
        }

        /// <summary>
        /// Updates a specific task fragment.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> UpdateFragment(string group, string fragmentId, EquipartitionTask.FragmentStates state)
        {
            var info = GetFragment(group, fragmentId);
            if (info.IsSelected) info.Parent.UpdateFragment(info.ItemSelected, state);
            return info;
        }
    }
}
