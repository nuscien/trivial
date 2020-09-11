using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Trivial.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The change event handler.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ChangeEventHandler<T>(object sender, ChangeEventArgs<T> e);

    /// <summary>
    /// The data event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> e);

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
    /// The event arguments about changing.
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
        /// <param name="autoMethod">true if set method automatically by value parameters; otherwise, false.</param>
        public ChangeEventArgs(T oldValue, T newValue, string key = null, bool autoMethod = false)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Key = key;
            if (!autoMethod) return;
            try
            {
                if (oldValue == null && newValue == null) Method = ChangeMethods.Same;
                else if (oldValue == null) Method = ChangeMethods.Remove;
                else if (newValue == null) Method = ChangeMethods.Add;
                else if (oldValue.Equals(newValue)) Method = ChangeMethods.Same;
            }
            catch (InvalidCastException)
            {
            }
            catch (InvalidOperationException)
            {
            }
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
        /// <param name="index">The index of the value in a list changed.</param>
        public ChangeEventArgs(T oldValue, T newValue, ChangeMethods method, int index) : this(oldValue, newValue, index.ToString("g", CultureInfo.InvariantCulture)) => Method = method;

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

    /// <summary>
    /// The event arguments with data.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    public class DataEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DataEventArgs class.
        /// </summary>
        /// <param name="data">The data.</param>
        public DataEventArgs(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public T Data { get; }
    }
}
