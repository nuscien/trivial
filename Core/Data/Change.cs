﻿using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The change event handler.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ChangeEventHandler<T>(object sender, ChangeEventArgs<T> e);

    /// <summary>
    /// The method to change.
    /// </summary>
    public enum ChangeMethods
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Unchanged.
        /// </summary>
        Unchanged = 1,

        /// <summary>
        /// The value has been updated but keep the same as original.
        /// </summary>
        Same = 2,

        /// <summary>
        /// Create or add.
        /// </summary>
        Add = 3,

        /// <summary>
        /// Replace.
        /// </summary>
        Update = 4,

        /// <summary>
        /// Delta update.
        /// </summary>
        MemberModify = 5,

        /// <summary>
        /// Delete.
        /// </summary>
        Remove = 6,

        /// <summary>
        /// The source is not valid any more.
        /// </summary>
        Invalid = 7
    }

    /// <summary>
    /// The event arguments with state.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    public class ChangeEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ChangeEventArgs class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="key">The property key of the value changed.</param>
        public ChangeEventArgs(T oldValue, T newValue, string key = null)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the ChangeEventArgs class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="triggerType">The type identifier of the trigger.</param>
        /// <param name="key">The property key of the value changed.</param>
        public ChangeEventArgs(T oldValue, T newValue, Guid triggerType, string key = null) : this(oldValue, newValue, key) => TriggerType = triggerType;

        /// <summary>
        /// Initializes a new instance of the ChangeEventArgs class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="method">The method to change.</param>
        /// <param name="key">The property key of the value changed.</param>
        public ChangeEventArgs(T oldValue, T newValue, ChangeMethods method, string key = null) : this(oldValue, newValue, key) => Method = method;

        /// <summary>
        /// Initializes a new instance of the ChangeEventArgs class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="method">The method to change.</param>
        /// <param name="triggerType">The type identifier of the trigger.</param>
        /// <param name="key">The property key of the value changed.</param>
        public ChangeEventArgs(T oldValue, T newValue, ChangeMethods method, Guid triggerType, string key = null) : this(oldValue, newValue, method, key) => TriggerType = triggerType;

        /// <summary>
        /// The method to change.
        /// </summary>
        public ChangeMethods Method { get; } = ChangeMethods.Update;

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public T NewValue { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the type identifier of the trigger.
        /// </summary>
        public Guid TriggerType { get; }
    }
}
