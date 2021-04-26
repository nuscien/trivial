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
    /// Equipartition fragment task and its thread-safe resolver.
    /// </summary>
    public partial class EquipartitionTask : IReadOnlyList<EquipartitionTask.Fragment>
    {
        private readonly object locker = new ();
        private readonly List<Fragment> fragments = new ();
        private string desc;

        /// <summary>
        /// Initializes a new instance of the EquipartitionTask class.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="count">The fragment count.</param>
        public EquipartitionTask(string jobId, int count)
        {
            JobId = jobId;
            for (var i = 0; i < count; i++)
            {
                fragments.Add(new Fragment(i));
            }
        }

        /// <summary>
        /// Initializes a new instance of the EquipartitionTask class.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="children">The fragments.</param>
        /// <param name="creation">The creation date time.</param>
        /// <param name="description">The description.</param>
        public EquipartitionTask(string id, string jobId, IEnumerable<Fragment> children, DateTime? creation = null, string description = null)
        {
            if (!string.IsNullOrWhiteSpace(id)) Id = id;
            JobId = jobId;
            desc = description;
            if (creation.HasValue) Creation = creation.Value;
            if (children != null) fragments.AddRange(children.Where(ele => ele != null && !string.IsNullOrWhiteSpace(ele.Id)));
        }

        /// <summary>
        /// Adds or removes the event occurs when the task fragment state or tag is changed.
        /// </summary>
        public event EventHandler<FragmentStateEventArgs> FragmentStateChanged;

        /// <summary>
        /// Adds or removes the event occurs when the task is done.
        /// </summary>
        public event EventHandler HasBeenDone;

        /// <summary>
        /// Adds or removes the event occurs when the description is changed.
        /// </summary>
        public event ChangeEventHandler<string> DescriptionChanged;

        /// <summary>
        /// Gets the task identifier.
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("n");

        /// <summary>
        /// Gets the job identifier.
        /// </summary>
        public string JobId { get; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description
        {
            get
            {
                return desc;
            }

            set
            {
                var oldValue = desc;
                desc = value;
                DescriptionChanged?.Invoke(this, new ChangeEventArgs<string>(oldValue, value, "Description", true));
            }
        }

        /// <summary>
        /// Gets the task fragment count.
        /// </summary>
        public int Count => fragments.Count;

        /// <summary>
        /// Gets a value indicating whether the task is done.
        /// </summary>
        public bool IsDone
        {
            get
            {
                return fragments.FirstOrDefault(ele =>
                {
                    return !ele.IsDone;
                }) == null;
            }
        }

        /// <summary>
        /// Gets the creation date time.
        /// </summary>
        public DateTime Creation { get; } = DateTime.Now;

        /// <summary>
        /// Gets the latest modification date time.
        /// </summary>
        public DateTime Modification
        {
            get
            {
                return fragments.Max(ele => ele.Modification);
            }
        }

        /// <summary>
        /// Gets the specific task fragment.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The fragment instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0, or equals to or greater than the length of fragments.</exception>
        public Fragment this[int index] => fragments[index];

        /// <summary>
        /// Gets the specific task fragment.
        /// </summary>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <returns>The fragment instance.</returns>
        /// <exception cref="ArgumentNullException">fragmentId was null.</exception>
        /// <exception cref="InvalidOperationException">No element matched.</exception>
        public Fragment this[string fragmentId] => fragments.First(ele => ele.Id == fragmentId);

        /// <summary>
        /// Tests if it contains such fragment identifier.
        /// </summary>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool ContainsId(string fragmentId) => fragments.FirstOrDefault(ele => ele.Id == fragmentId) != null;

        /// <summary>
        /// Gets the specific task fragment; or null if no such element.
        /// </summary>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <returns>The fragment instance.</returns>
        public Fragment TryGetByFragmentId(string fragmentId) => fragments.FirstOrDefault(ele => ele.Id == fragmentId);

        /// <summary>
        /// Gets the specific task fragment; or null if no such element.
        /// </summary>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <param name="value">The fragment instance.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool TryGetByFragmentId(string fragmentId, out Fragment value)
        {
            value = TryGetByFragmentId(fragmentId);
            return value != null;
        }

        /// <summary>
        /// Gets the collection of the task fragment by its index.
        /// </summary>
        public IEnumerable<Fragment> GetByFragmentIndex(int index) => fragments.Where(ele => ele.Index == index);

        /// <summary>
        /// Gets the collection of the task fragment which is working or retrying.
        /// </summary>
        public IEnumerable<Fragment> GetProcessingFragments() => fragments.Where(ele => ele.IsProcessing);

        /// <summary>
        /// Gets the collection of the task fragment which is not done yet.
        /// </summary>
        public IEnumerable<Fragment> GetWaitingOrProcessingFragments() => fragments.Where(ele => !ele.IsDone);

        /// <summary>
        /// Gets the collection of the task fragment which is done (even if it is any of success, fatal or ignored).
        /// </summary>
        public IEnumerable<Fragment> GetDoneFragments() => fragments.Where(ele => ele.IsDone);

        /// <summary>
        /// Gets the collection of the task fragment which is pending or failure.
        /// </summary>
        public IEnumerable<Fragment> GetWaitingFragments() => fragments.Where(ele => ele.IsWaiting);

        /// <summary>
        /// Gets the collection of the task fragment which is fatal or failure (even if it is either waiting or done).
        /// </summary>
        public IEnumerable<Fragment> GetErrorFragments() => fragments.Where(ele => ele.IsError);

        /// <summary>
        /// Gets the collection of the task fragment which is done but not successful (even if it is either waiting or done).
        /// </summary>
        public IEnumerable<Fragment> GetErrorOrIngoredFragments() => fragments.Where(ele => ele.IsErrorOrIgnored);

        /// <summary>
        /// Filters a sequence of values with specific state.
        /// </summary>
        /// <param name="state">Specific state.</param>
        /// <param name="states">Additional state grouped by OR boolean operation.</param>
        /// <returns>An collection that contains elements from the fragments that satisfy the condition.</returns>
        public IEnumerable<Fragment> Where(FragmentStates state, params FragmentStates[] states)
        {
            return states == null || states.Length == 0
                ? fragments.Where(ele => ele.State == state)
                : fragments.Where(ele => ele.State == state || states.Contains(ele.State));
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="state">The task fragment state expected.</param>
        /// <returns>A fragment.</returns>
        public Fragment Pick(FragmentStates state) => Pick(null, state, null);

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A fragment.</returns>
        public Fragment Pick(IEnumerable<string> except = null, bool onlyPending = false)
        {
            return onlyPending ? Pick(except, FragmentStates.Pending, null) : Pick(except, null, null);
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="tag">New tag.</param>
        /// <param name="state">The task fragment state expected.</param>
        /// <returns>A fragment.</returns>
        public Fragment Pick(string tag, FragmentStates state) => Pick(null, state, fragment => fragment.Tag = tag);

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="tag">New tag.</param>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A fragment.</returns>
        public Fragment Pick(string tag, IEnumerable<string> except = null, bool onlyPending = false)
        {
            return onlyPending ? Pick(except, FragmentStates.Pending, fragment => fragment.Tag = tag) : Pick(except, null, fragment => fragment.Tag = tag);
        }

        /// <summary>
        /// Updates the task fragment.
        /// </summary>
        /// <param name="id">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <param name="tag">The new tag.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public bool UpdateFragment(string id, FragmentStates? state, string tag) => UpdateFragment(id, state, f =>
        {
            f.Tag = tag;
        });

        /// <summary>
        /// Updates the task fragment.
        /// </summary>
        /// <param name="id">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public bool UpdateFragment(string id, FragmentStates state) => UpdateFragment(id, state, null as Action<Fragment>);

        /// <summary>
        /// Updates the task fragment.
        /// </summary>
        /// <param name="fragment">The task fragment instance.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <param name="tag">The new tag.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public bool UpdateFragment(Fragment fragment, FragmentStates? state, string tag) => UpdateFragment(fragment?.Id, state, f =>
        {
            f.Tag = tag;
            if (fragment == f) return;
            fragment.Modification = f.Modification;
            fragment.State = f.State;
            fragment.Tag = f.Tag;
        });

        /// <summary>
        /// Updates the task fragment.
        /// </summary>
        /// <param name="fragment">The task fragment instance.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public bool UpdateFragment(Fragment fragment, FragmentStates state) => UpdateFragment(fragment?.Id, state, f =>
        {
            if (fragment == f) return;
            fragment.Modification = f.Modification;
            fragment.State = f.State;
            fragment.Tag = f.Tag;
        });

        /// <summary>
        /// Cancels.
        /// </summary>
        public void Cancel()
        {
            List<(Fragment, FragmentStates)> col;
            lock (locker)
            {
                col = GetWaitingOrProcessingFragments().Select(ele => (ele, ele.State)).ToList();
                if (col.Count == 0) return;
                foreach (var fragment in col)
                {
                    fragment.Item1.State = FragmentStates.Ignored;
                    fragment.Item1.Modification = DateTime.Now;
                }
            }

            foreach (var fragment in col)
            {
                FragmentStateChanged?.Invoke(this, new FragmentStateEventArgs(fragment.Item1, fragment.Item2));
            }

            HasBeenDone?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Gets the JSON format string serialized of this object.
        /// </summary>
        /// <returns>A JSON format string.</returns>
        public string ToJsonString() => ToJsonString(true);

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <param name="containFragments">true if output all the fragments; otherwise, false.</param>
        /// <param name="additionalFragment">The additional fragment to output.</param>
        /// <param name="additionalProperties">The additional JSON string properties to output.</param>
        /// <returns>A JSON format string.</returns>
        public string ToJsonString(bool containFragments, Fragment additionalFragment = null, IDictionary<string, string> additionalProperties = null)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "\"id\":{0},\"job\":{1},\"creation\":{2},\"update\":{3}\"done\":{4}",
                JsonString.ToJson(Id),
                JsonString.ToJson(JobId),
                WebFormat.ParseDate(Creation),
                WebFormat.ParseDate(Modification),
                IsDone);
            if (desc != null) sb.Append($",\"desc\":{JsonString.ToJson(desc)}");
            if (additionalFragment != null) sb.Append($",\"fragment\":{additionalFragment.ToJsonString()}");
            if (containFragments)
            {
                var jArr = new List<string>();
                foreach (var fragment in fragments)
                {
                    if (fragment != null) jArr.Add(fragment.ToJsonString());
                }

                sb.AppendFormat(",\"fragments\":[{0}]", string.Join(",", jArr));
            }

            if (additionalProperties != null)
            {
                foreach (var kvp in additionalProperties)
                {
                    sb.AppendFormat(",{0}:{1}", JsonString.ToJson(kvp.Key), JsonString.ToJson(kvp.Value));
                }
            }

            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("Task ID: {0}. Job ID: {1}. Fragment count: {2}.", Id, JobId, Count);
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="except">The except task fragment identifier.</param>
        /// <param name="state">The task fragment state expected.</param>
        /// <param name="callback">The success callback.</param>
        /// <returns>A fragment.</returns>
        private Fragment Pick(IEnumerable<string> except, FragmentStates? state, Action<Fragment> callback)
        {
            Fragment fragment = null;
            FragmentStates oldState = FragmentStates.Pending;
            var wasDone = IsDone;
            lock (locker)
            {
                IEnumerable<Fragment> list = fragments;
                if (except != null) list = list.Where(ele =>
                {
                    return !except.Contains(ele.Id);
                });
                fragment = state.HasValue ? list.FirstOrDefault(ele =>
                {
                    return ele.State == state;
                }) : list.FirstOrDefault(ele =>
                {
                    return ele.State == FragmentStates.Pending || ele.State == FragmentStates.Failure;
                });
                if (fragment == null) return null;
                switch (oldState = fragment.State)
                {
                    case FragmentStates.Pending:
                        fragment.State = FragmentStates.Working;
                        fragment.Modification = DateTime.Now;
                        break;
                    case FragmentStates.Failure:
                        fragment.State = FragmentStates.Retrying;
                        fragment.Modification = DateTime.Now;
                        break;
                    default:
                        if (state.HasValue) break;
                        return null;
                }

                if (callback != null)
                {
                    fragment.Modification = DateTime.Now;
                    callback(fragment);
                }
            }

            NotifyChange(fragment, oldState, wasDone);
            return fragment;
        }

        /// <summary>
        /// Updates the task fragment.
        /// </summary>
        /// <param name="id">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <param name="callback">The success callback.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        private bool UpdateFragment(string id, FragmentStates? state, Action<Fragment> callback)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var fragment = TryGetByFragmentId(id);
            if (fragment == null) return false;
            var oldState = fragment.State;
            if (!state.HasValue)
            {
                if (callback != null)
                {
                    fragment.Modification = DateTime.Now;
                    callback(fragment);
                    FragmentStateChanged?.Invoke(this, new FragmentStateEventArgs(fragment, oldState));
                }

                return true;
            }

            var wasDone = IsDone;
            lock (locker)
            {
                if (state != oldState)
                {
                    if (oldState == FragmentStates.Success || oldState == FragmentStates.Fatal || oldState == FragmentStates.Ignored) return false;
                    switch (state.Value)
                    {
                        case FragmentStates.Pending:
                            if (oldState == FragmentStates.Working) break;
                            return false;
                        case FragmentStates.Working:
                        case FragmentStates.Retrying:
                            state = oldState == FragmentStates.Pending ? FragmentStates.Working : FragmentStates.Retrying;
                            break;
                    }

                    fragment.State = state.Value;
                    fragment.Modification = DateTime.Now;
                }
                else
                {
                    fragment.Modification = DateTime.Now;
                }

                callback?.Invoke(fragment);
            }

            NotifyChange(fragment, oldState, wasDone);
            return true;
        }

        private void NotifyChange(Fragment fragment, FragmentStates oldState, bool wasDone)
        {
            if (fragment == null) return;
            FragmentStateChanged?.Invoke(this, new FragmentStateEventArgs(fragment, oldState));
            if (!wasDone && IsDone) HasBeenDone?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        public IEnumerator<Fragment> GetEnumerator()
        {
            return fragments.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return fragments.GetEnumerator();
        }
    }
}
