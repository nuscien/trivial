using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Reflection;

/// <summary>
/// The base linked nested parameter.
/// </summary>
/// <param name="parameter">The parameter.</param>
public abstract class BaseNestedParameter(object parameter)
{
    /// <summary>
    /// Gets the parameter.
    /// </summary>
    public object Parameter { get; } = parameter;

    /// <summary>
    /// Tries to convert the value in a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value converted.</param>
    /// <returns>true if the type is the specific one; otherwise, false.</returns>
    public bool ParameterIs<T>(out T value)
        => ParameterIs(10, out value);

    /// <summary>
    /// Tries to convert the value in a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="maxRecurrence">The maximum recurrence count.</param>
    /// <param name="value">The value converted.</param>
    /// <returns>true if the type is the specific one; otherwise, false.</returns>
    public bool ParameterIs<T>(int maxRecurrence, out T value)
    {
        if (ObjectConvert.TryGetForSimple(Parameter, out value)) return true;
        if (maxRecurrence < 1) return false;
        if (Parameter is BaseNestedParameter n) return n.ParameterIs(maxRecurrence - 1, out value);
        if (Parameter is TypedNestedParameter t) return t.TryGet(out value);
        value = default;
        return false;
    }
}

/// <summary>
/// The typed nested parameter class.
/// </summary>
public class TypedNestedParameter
{
    private readonly Dictionary<Type, object> store;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedNestedParameter"/> class.
    /// </summary>
    public TypedNestedParameter()
    {
        store = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedNestedParameter"/> class.
    /// </summary>
    /// <param name="copy">The instance to copy its registry for current state.</param>
    public TypedNestedParameter(TypedNestedParameter copy)
    {
        store = new(copy.store);
    }

    /// <summary>
    /// Gets all types registered.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IReadOnlyCollection<Type> TypesRegistered => store.Keys;

    /// <summary>
    /// Registers the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to register.</param>
    public void Register<T>(T value)
        => store[typeof(T)] = value;

    /// <summary>
    /// Registers the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to register.</param>
    public void Register<T>(IObjectRef<T> value)
        => store[typeof(T)] = value;

    /// <summary>
    /// Registers the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to register.</param>
    public void Register<T>(Lazy<T> value)
        => store[typeof(T)] = value;

    /// <summary>
    /// Registers the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to register.</param>
    public void Register<T>(IObjectResolver<T> value)
        => store[typeof(T)] = value;

    /// <summary>
    /// Removes the specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the registry.</returns>
    public bool Remove<T>()
        => store.Remove(typeof(T));

    /// <summary>
    /// Removes the specific type.
    /// </summary>
    /// <param name="type">The type of value.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the registry.</returns>
    public bool Remove(Type type)
        => type != null && store.Remove(type);

    /// <summary>
    /// Removes the specific type.
    /// </summary>
    /// <param name="types">The types to remove.</param>
    /// <returns>The count of item removed.</returns>
    public int Remove(IEnumerable<Type> types)
    {
        var i = 0;
        if (types == null) return i;
        foreach (var type in types)
        {
            if (Remove(type)) i++;
        }

        return i;
    }

    /// <summary>
    /// Tests whether the value of a specific type is registered.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool Contains<T>()
        => store.ContainsKey(typeof(T));

    /// <summary>
    /// Tests whether the value of a specific type is registered.
    /// </summary>
    /// <param name="type">The type of value.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool Contains(Type type)
        => type != null && store.ContainsKey(type);

    /// <summary>
    /// Gets the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <returns>The value resolved.</returns>
    /// <exception cref="KeyNotFoundException">No value registered.</exception>
    /// <exception cref="NotSupportedException">Cannot resolve the object.</exception>
    public T Get<T>()
    {
        if (store.TryGetValue(typeof(T), out var value) && value is not null)
        {
            const string errorMessage = "Cannot resolve the object.";
            try
            {
                if (value is T o) return o;
                if (value is IObjectRef<T> r) return r.Value;
                if (value is Lazy<T> l) return l.Value;
                if (value is IObjectResolver<T> h) return h.GetInstance();
            }
            catch (MemberAccessException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (NullReferenceException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (ArgumentException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (KeyNotFoundException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (FormatException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (OverflowException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (ArithmeticException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (NotImplementedException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
            catch (ExternalException ex)
            {
                throw new NotSupportedException(errorMessage, ex);
            }
        }

        throw new KeyNotFoundException($"No value registered for type {typeof(T).FullName}.");
    }

    /// <summary>
    /// Tries to get the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value resolved.</param>
    /// <returns>true if has; otherwise, false.</returns>
    public bool TryGet<T>(out T value)
    {
        if (store.TryGetValue(typeof(T), out var v))
        {
            if (ObjectConvert.TryGetForSimple(v, out value)) return true;
            if (v is BaseNestedParameter n) return ObjectConvert.TryGetForSimple(n.Parameter, out value);
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="defaultValue">The fallback value if not found.</param>
    /// <returns>The value resolved.</returns>
    public T TryGet<T>(T defaultValue = default)
        => TryGet(out T value) ? value : defaultValue;
}
