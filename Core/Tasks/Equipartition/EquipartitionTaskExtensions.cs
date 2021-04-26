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
    /// The equipartition task collection.
    /// </summary>
    public static class EquipartitionTaskExtensions
    {
        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="pick">A handler to pick.</param>
        /// <returns>A task and fragment.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(this IEnumerable<EquipartitionTask> col, Func<EquipartitionTask, EquipartitionTask.Fragment> pick = null)
        {
            return Pick(col, null, pick);
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A task and fragment.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(this IEnumerable<EquipartitionTask> col, string tag, IEnumerable<string> except = null, bool onlyPending = false)
        {
            return Pick(col, null, task => task.Pick(tag, except, onlyPending));
        }

        /// <summary>
        /// Picks one and start.
        /// </summary>
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pick">A handler to pick.</param>
        /// <returns>A task and fragment.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(this IEnumerable<EquipartitionTask> col, Func<EquipartitionTask, bool> predicate, Func<EquipartitionTask, EquipartitionTask.Fragment> pick = null)
        {
            if (col == null) return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>();
            if (predicate != null) col = col.Where(predicate);
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
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="except">The fragment identifier except.</param>
        /// <param name="onlyPending">true if get only pending one; otherwise, false.</param>
        /// <returns>A task and fragment.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> Pick(this IEnumerable<EquipartitionTask> col, Func<EquipartitionTask, bool> predicate, string tag, IEnumerable<string> except = null, bool onlyPending = false)
        {
            return Pick(col, predicate, task => task.Pick(tag, except, onlyPending));
        }

        /// <summary>
        /// Gets a task fragment.
        /// </summary>
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> GetFragment(this IEnumerable<EquipartitionTask> col, string fragmentId)
        {
            if (col == null) return new SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment>();
            foreach (var task in col)
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
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <param name="tag">The new tag.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> UpdateFragment(this IEnumerable<EquipartitionTask> col, string fragmentId, EquipartitionTask.FragmentStates? state, string tag)
        {
            var info = GetFragment(col, fragmentId);
            if (info.IsSelected) info.Parent.UpdateFragment(info.ItemSelected, state, tag);
            return info;
        }

        /// <summary>
        /// Updates a specific task fragment.
        /// </summary>
        /// <param name="col">The equipartition task collection.</param>
        /// <param name="fragmentId">The task fragment identifier.</param>
        /// <param name="state">The new state; or null if no change.</param>
        /// <returns>true if update succeeded; otherwise, false.</returns>
        public static SelectionRelationship<EquipartitionTask, EquipartitionTask.Fragment> UpdateFragment(this IEnumerable<EquipartitionTask> col, string fragmentId, EquipartitionTask.FragmentStates state)
        {
            var info = GetFragment(col, fragmentId);
            if (info.IsSelected) info.Parent.UpdateFragment(info.ItemSelected, state);
            return info;
        }
    }
}
