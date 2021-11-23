using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Reflection
{
    /// <summary>
    /// Object reference interface.
    /// </summary>
    public interface IObjectRef
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// Object reference interface.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public interface IObjectRef<T> : IObjectRef
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        new T Value { get; }
    }

    /// <summary>
    /// Object reference.
    /// </summary>
    public sealed class ObjectRef : IObjectRef
    {
        private readonly IObjectRef reference;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(IObjectRef value) => reference = value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public ObjectRef(Func<object> factory) => reference = new FactoryObjectRef(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(object value) => reference = new InstanceObjectRef(value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value => reference.Value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public static IObjectRef Create(Func<object> factory) => new FactoryObjectRef(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public static IObjectRef Create(object value) => new InstanceObjectRef(value);
    }

    /// <summary>
    /// Object reference.
    /// </summary>
    public sealed class ObjectRef<T> : IObjectRef<T>, IObjectRef
    {
        private readonly IObjectRef<T> reference;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(IObjectRef<T> value) => reference = value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public ObjectRef(Func<T> factory) => reference = new FactoryObjectRef<T>(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public ObjectRef(Lazy<T> lazy) => reference = new LazyObjectRef<T>(lazy);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(T value) => reference = new InstanceObjectRef<T>(value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value => reference.Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => reference.Value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public static IObjectRef<T> Create(Func<T> factory) => new FactoryObjectRef<T>(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public static IObjectRef<T> Create(Lazy<T> lazy) => new LazyObjectRef<T>(lazy);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public static IObjectRef<T> Create(T value) => new InstanceObjectRef<T>(value);
    }

    /// <summary>
    /// Instance object reference.
    /// </summary>
    internal class InstanceObjectRef : IObjectRef
    {
        /// <summary>
        /// Initializes a new instance of the InstanceObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public InstanceObjectRef(object value) => Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }
    }

    /// <summary>
    /// Instance object reference.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class InstanceObjectRef<T> : InstanceObjectRef, IObjectRef<T>
    {
        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public InstanceObjectRef(T value) : base(value) => Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public new T Value { get; }
    }

    /// <summary>
    /// Object reference for lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class LazyObjectRef<T> : IObjectRef, IObjectRef<T>
    {
        private readonly Lazy<T> value;

        /// <summary>
        /// Initializes a new instance of the LazyObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public LazyObjectRef(Lazy<T> lazy) => value = lazy;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value => value.Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => value.Value;
    }

    /// <summary>
    /// Object reference for thread safe factory.
    /// </summary>
    internal class FactoryObjectRef : IObjectRef
    {
        private readonly object locker = new ();
        private readonly Func<object> f;
        private bool isInit;
        private object value;

        /// <summary>
        /// Initializes a new instance of the FactoryObjectRef class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FactoryObjectRef(Func<object> factory) => f = factory;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value
        {
            get
            {
                if (isInit) return value;
                lock (locker)
                {
                    if (isInit) return value;
                    isInit = true;
                    return value = f();
                }
            }
        }
    }

    /// <summary>
    /// Object reference for thread safe factory.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class FactoryObjectRef<T> : IObjectRef, IObjectRef<T>
    {
        private readonly object locker = new ();
        private readonly Func<T> f;
        private bool isInit;
        private T value;

        /// <summary>
        /// Initializes a new instance of the FactoryObjectRef class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FactoryObjectRef(Func<T> factory) => f = factory;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get
            {
                if (isInit) return value;
                lock (locker)
                {
                    if (isInit) return value;
                    isInit = true;
                    return value = f();
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => Value;
    }

    /// <summary>
    /// A model with the relationship between the item selected and its parent.
    /// </summary>
    public class SelectionRelationship<TParent, TItem>
    {
        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        public SelectionRelationship()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="tuple">The parent and item selected.</param>
        public SelectionRelationship((TParent, TItem) tuple)
        {
            IsSelected = true;
            Parent = tuple.Item1;
            ItemSelected = tuple.Item2;
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SelectionRelationship(TParent parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="itemSelected">The item selected.</param>
        public SelectionRelationship(TParent parent, TItem itemSelected)
        {
            IsSelected = true;
            Parent = parent;
            ItemSelected = itemSelected;
        }

        /// <summary>
        /// Gets a value indicating whether there is an item selected.
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Gets the parent which contains the item.
        /// </summary>
        public TParent Parent { get; }

        /// <summary>
        /// Gets the item selected.
        /// </summary>
        public TItem ItemSelected { get; }

        /// <summary>
        /// Converts to tuple.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <return>The tuple.</return>
        public static explicit operator (TParent, TItem, bool)(SelectionRelationship<TParent, TItem> value)
        {
            return value != null ? (value.Parent, value.ItemSelected, value.IsSelected) : (default(TParent), default(TItem), false);
        }
    }

    /// <summary>
    /// The version comparer.
    /// </summary>
    public class VersionComparer : IComparer<string>
    {
        /// <summary>
        /// Gets a value indicating whether x is wide version scope.
        /// </summary>
        public bool IsWideX { get; set; }

        /// <summary>
        /// Compares two versions and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first version to compare.</param>
        /// <param name="y">The second version to compare.</param>
        /// <returns>
        /// <para>
        /// A signed integer that indicates the relative values of x and y, as shown in the following.
        /// </para>
        /// <list type="bullet">
        /// <item>Value Meaning Less than zero x is less than y.</item>
        /// <item>Zero x equals y.</item>
        /// <item>Greater than zero x is greater than y.</item>
        /// </list>
        /// </returns>
        public int Compare(string x, string y)
        {
            return Compare(x, y, IsWideX);
        }

        /// <summary>
        /// Converts the item to version.
        /// </summary>
        /// <param name="s">the version string.</param>
        /// <returns>A version instance.</returns>
        public static Version ToVersion(string s)
        {
            #pragma warning disable IDE0057
            s = s?.Trim();
            if (string.IsNullOrEmpty(s)) return null;
            var end = s.IndexOf('-');
            if (end >= 0) s = s.Substring(0, end);
            end = s.IndexOf("+");
            if (end >= 0) s = s.Substring(0, end);
            while (s.EndsWith(".*")) s = s.Substring(0, s.Length - 2);
            if (s.Length == 0 || s == "*") return null;
            var arr = s.Split('.');
            if (!int.TryParse(arr[0], out var major)) return null;
            if (arr.Length < 2 || !int.TryParse(arr[1], out var minor)) return new Version(major, 0);
            if (arr.Length < 3 || !int.TryParse(arr[2], out var build)) return new Version(major, minor);
            if (arr.Length < 4 || !int.TryParse(arr[3], out var rev)) return new Version(major, minor, build);
            return new Version(major, minor, build, rev);
            #pragma warning restore IDE0057
        }

        /// <summary>
        /// Compares two versions and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first version to compare.</param>
        /// <param name="y">The second version to compare.</param>
        /// <param name="isWideX">true if x is wide version scope; otherwise, false.</param>
        /// <returns>
        /// <para>
        /// A signed integer that indicates the relative values of x and y, as shown in the following.
        /// </para>
        /// <list type="bullet">
        /// <item>Value Meaning Less than zero x is less than y.</item>
        /// <item>Zero x equals y.</item>
        /// <item>Greater than zero x is greater than y.</item>
        /// </list>
        /// </returns>
        public static int Compare(string x, string y, bool isWideX)
        {
            #pragma warning disable IDE0057
            x = x?.Trim();
            y = y?.Trim();
            var r = 1;
            if (string.IsNullOrEmpty(y) || y.StartsWith("+")) return r;
            if (string.IsNullOrEmpty(x)) return 0;
            if (x.StartsWith("+")) return -r;
            while (x.EndsWith(".*"))
            {
                x = x.Substring(0, x.Length - 2);
                isWideX = true;
            }

            if (x.Length == 0 || x == "*") return 0;
            var leftMetaPos = x.IndexOf("+");
            if (leftMetaPos > 0) x = x.Substring(0, leftMetaPos);
            var rightMetaPos = y.IndexOf("+");
            if (rightMetaPos > 0) y = y.Substring(0, rightMetaPos);
            var leftArr = x.Split('-');
            var rightArr = y.Split('-');
            var arrLen = Math.Min(leftArr.Length, rightArr.Length);
            for (var j = 0; j < arrLen; j++)
            {
                var a = leftArr[j].Split('.');
                var b = rightArr[j].Split('.');
                var len = Math.Min(a.Length, b.Length);
                for (var i = 0; i < len; i++)
                {
                    try
                    {
                        r++;
                        var aPart = a[i]?.Trim();
                        if (string.IsNullOrEmpty(aPart)) aPart = "0";
                        var bPart = b[i]?.Trim();
                        if (string.IsNullOrEmpty(bPart)) bPart = "0";
                        if (aPart == bPart) continue;
                        if (int.TryParse(aPart, out var aNum) && int.TryParse(bPart, out var bNum))
                        {
                            if (aNum == bNum) continue;
                            return aNum < bNum ? -r : r;
                        }

                        return aPart.CompareTo(bPart) < 0 ? -r : r;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                if (isWideX && a.Length <= b.Length && j == arrLen - 1) return 0;
                if (a.Length != b.Length) return a.Length < b.Length ? -r : r;
            }

            if (leftArr.Length == y.Length) return 0;
            return leftArr.Length > y.Length ? -r : r;
            #pragma warning restore IDE0057
        }
    }
}
