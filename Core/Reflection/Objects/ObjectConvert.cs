using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Trivial.Data;
using Trivial.Text;

namespace Trivial.Reflection;

/// <summary>
/// The type utility.
/// </summary>
public static class ObjectConvert
{
    /// <summary>
    /// Converts the given object to a specific struct.
    /// </summary>
    /// <param name="type">The type of object to return.</param>
    /// <param name="value">The boxed to convert.</param>
    /// <returns>An object converted.</returns>
    /// <exception cref="InvalidCastException">Failed to convert.</exception>
    /// <exception cref="FormatException">boxed is not in a format recognized by conversion type.</exception>
    /// <exception cref="ArgumentNullException">type or boxed was null.</exception>
    /// <exception cref="ArgumentException">boxed was not the one in the type or content format supported.</exception>
    /// <exception cref="OverflowException">boxed was outside the range of the underlying type of the specific type to convert.</exception>
    public static object Invoke(Type type, object value)
    {
        if (type == null) throw ArgumentNull(nameof(type));
        if (type == typeof(string))
        {
            if (value is Stream streamValue)
            {
                using var reader = new StreamReader(streamValue);
                return reader.ReadToEnd();
            }

            return Convert.ToString(value);
        }

        if (value == null)
        {
            if (type.IsClass || IsNullableValueType(type)) return null;
            throw ArgumentNull(nameof(value));
        }

        var objectType = value.GetType();
        if (objectType.IsValueType || objectType.IsEnum) return Convert.ChangeType(value, type);
        if (value is string str)
        {
            if (type == typeof(string)) return str;
            else if (type.IsEnum)
            {
                return Enum.Parse(type, str);
            }
            else if (type.IsValueType)
            {
                Text.StringExtensions.AssertNotWhiteSpace(nameof(value), str);
                if (type == typeof(bool))
                    return Text.JsonBooleanNode.Parse(str);
                if (type == typeof(int))
                    return Maths.Numbers.ParseToInt32(str, 10);
                if (type == typeof(long))
                    return Maths.Numbers.ParseToInt64(str, 10);
                if (type == typeof(short))
                    return Maths.Numbers.ParseToInt16(str, 10);
                if (type == typeof(double))
                    return double.Parse(str);
                if (type == typeof(float))
                    return float.Parse(str);
                if (type == typeof(decimal))
                    return decimal.Parse(str);
                if (type == typeof(uint))
                    return Maths.Numbers.ParseToUInt32(str, 10);
                if (type == typeof(ushort))
                    return Maths.Numbers.ParseToUInt16(str, 10);
                if (type == typeof(Guid))
                    return Guid.Parse(str);
#if NET8_0_OR_GREATER
                if (type == typeof(Int128))
                    return Maths.Numbers.ParseToInt128(str, 10);
#endif
            }
            else if (type == typeof(StringBuilder))
            {
                return new StringBuilder(str);
            }
            else if (type == typeof(Uri))
            {
                return new Uri(str, UriKind.RelativeOrAbsolute); ;
            }
            else if (type == typeof(JsonEncodedText))
            {
                return JsonEncodedText.Encode(str);
            }
            else if (type == typeof(Text.JsonStringNode))
            {
                return new Text.JsonStringNode(str);
            }

            try
            {
                var parser = type.GetMethod("Parse", new[] { typeof(string) });
                if (parser != null && parser.IsStatic && !parser.IsAbstract)
                {
                    return parser.Invoke(null, new[] { str });
                }
            }
            catch (ArgumentException)
            {
            }
            catch (AmbiguousMatchException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (MemberAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (TargetInvocationException)
            {
            }
            catch (TargetException)
            {
            }

            if (type == typeof(Stream) || type.IsSubclassOf(typeof(Stream)))
            {
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                }

                return stream;
            }
        }

        return Convert.ChangeType(value, type);
    }

    /// <summary>
    /// Converts the given object to a specific struct.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="value">The boxed to convert.</param>
    /// <returns>An object converted.</returns>
    /// <exception cref="InvalidCastException">Failed to convert.</exception>
    /// <exception cref="ArgumentNullException">boxed is null.</exception>
    /// <exception cref="ArgumentException">boxed is not the one in the type or content format supported.</exception>
    /// <exception cref="OverflowException">boxed is outside the range of the underlying type of the specific type to convert.</exception>
    public static T Invoke<T>(object value)
        => (T)Invoke(typeof(T), value);

    /// <summary>
    /// Tries to convert the given object to a specific struct.
    /// </summary>
    /// <typeparam name="T">The type of the boxed type instance to return.</typeparam>
    /// <param name="value">The boxed to convert.</param>
    /// <returns>An struct converted.</returns>
    public static T? TryInvokeForStruct<T>(object value) where T : struct
    {
        if (value == null) return null;
        try
        {
            return Invoke<T>(value);
        }
        catch (InvalidCastException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (FormatException)
        {
        }
        catch (IOException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to convert the given object to a specific struct.
    /// </summary>
    /// <typeparam name="T">The type of reference type instance to return.</typeparam>
    /// <param name="value">The boxed to convert.</param>
    /// <returns>An struct converted.</returns>
    public static T TryInvokeForClass<T>(object value) where T : class
    {
        if (value == null) return null;
        try
        {
            return Invoke<T>(value);
        }
        catch (InvalidCastException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (FormatException)
        {
        }
        catch (IOException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to convert the given object to a specific struct.
    /// </summary>
    /// <typeparam name="T">The type of the boxed type instance to return.</typeparam>
    /// <param name="value">The boxed to convert.</param>
    /// <param name="output">The result output.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryInvokeForStruct<T>(object value, out T output) where T : struct
    {
        if (value == null && IsNullableValueType(typeof(T)))
        {
            output = default;
            return true;
        }

        var result = TryInvokeForStruct<T>(value);
        if (!result.HasValue)
        {
            output = default;
            return false;
        }

        output = result.Value;
        return true;
    }

    /// <summary>
    /// Tries to convert the given object to a specific struct.
    /// </summary>
    /// <typeparam name="T">The type of the boxed type instance to return.</typeparam>
    /// <param name="value">The boxed to convert.</param>
    /// <param name="output">The result output.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryInvokeForClass<T>(object value, out T output) where T : class
    {
        try
        {
            output = Invoke<T>(value);
            return true;
        }
        catch (InvalidCastException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (FormatException)
        {
        }
        catch (IOException)
        {
        }
        catch (NullReferenceException)
        {
        }

        output = null;
        return false;
    }

    /// <summary>
    /// Returns the typed instance.
    /// </summary>
    /// <typeparam name="T">The type of each instance in the collection.</typeparam>
    /// <param name="fields">The field values.</param>
    /// <param name="creator">The instance factory.</param>
    /// <param name="propertyNames">The optional property names to map.</param>
    /// <returns>A typed instance based on the fields.</returns>
    public static T Invoke<T>(IReadOnlyList<string> fields, Func<IReadOnlyList<string>, T> creator, IEnumerable<string> propertyNames = null)
    {
        var type = typeof(T);
        var props = propertyNames?.Select(ele =>
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ele)) return type.GetProperty(ele);
            }
            catch (AmbiguousMatchException)
            {
            }

            return null;
        })?.ToList();
        if (props != null && props.Count == 0) props = null;
        return Invoke(fields, creator, props);
    }

    /// <summary>
    /// Returns the typed instance.
    /// </summary>
    /// <typeparam name="T">The type of each instance in the collection.</typeparam>
    /// <param name="fields">The field values.</param>
    /// <param name="propertyNames">The optional property names to map.</param>
    /// <returns>A typed instance based on the fields.</returns>
    public static T Invoke<T>(IReadOnlyList<string> fields, IEnumerable<string> propertyNames)
    {
        return Invoke<T>(fields, null, propertyNames);
    }

    /// <summary>
    /// Returns the typed instance.
    /// </summary>
    /// <typeparam name="T">The type of each instance in the collection.</typeparam>
    /// <param name="fields">The field values.</param>
    /// <param name="creator">The instance factory.</param>
    /// <param name="properties">The optional properties to map.</param>
    /// <returns>A typed instance based on the fields.</returns>
    public static T Invoke<T>(IReadOnlyList<string> fields, Func<IReadOnlyList<string>, T> creator, IReadOnlyList<PropertyInfo> properties)
    {
        var instance = creator != null ? creator(fields) : Activator.CreateInstance<T>();
        if (instance == null) return default;
        if (properties != null)
        {
            for (var i = 0; i < Math.Min(properties.Count, fields.Count); i++)
            {
                var prop = properties[i];
                if (prop == null || !prop.CanWrite) continue;
                var propV = Invoke(prop.PropertyType, fields[i]);
                prop.SetValue(instance, propV);
            }
        }

        return instance;
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="data">The data.</param>
    /// <param name="message">The additional message.</param>
    public static void Invoke<T>(this DataEventHandler<T> handler, object sender, T data, string message = null)
    {
        if (handler == null) return;
        var args = new DataEventArgs<T>(data, message);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of boxed</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The boxed.</param>
    public static void Invoke<TKey, TValue>(this KeyValueEventHandler<TKey, TValue> handler, object sender, TKey key, TValue value)
    {
        if (handler == null) return;
        var args = new KeyValueEventArgs<TKey, TValue>(key, value);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="TSource">The type of source.</typeparam>
    /// <typeparam name="TKey">The type of property key.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="source">The source</param>
    /// <param name="key">The property key.</param>
    public static void Invoke<TSource, TKey>(this SourcePropertyEventHandler<TSource, TKey> handler, object sender, TSource source, TKey key)
    {
        if (handler == null) return;
        var args = new SourcePropertyEventArgs<TSource, TKey>(source, key);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old boxed.</param>
    /// <param name="newValue">The new boxed.</param>
    /// <param name="method">The method to change.</param>
    /// <param name="key">The property key of the boxed changed.</param>
    public static void Invoke<T>(this ChangeEventHandler<T> handler, object sender, T oldValue, T newValue, ChangeMethods method, string key = null)
    {
        if (handler == null) return;
        var args = new ChangeEventArgs<T>(oldValue, newValue, method, key);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old boxed.</param>
    /// <param name="newValue">The new boxed.</param>
    /// <param name="method">The method to change.</param>
    /// <param name="triggerType">The type identifier of the trigger.</param>
    /// <param name="key">The property key of the boxed changed.</param>
    public static void Invoke<T>(this ChangeEventHandler<T> handler, object sender, T oldValue, T newValue, ChangeMethods method, Guid triggerType, string key = null)
    {
        if (handler == null) return;
        var args = new ChangeEventArgs<T>(oldValue, newValue, method, triggerType, key);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old boxed.</param>
    /// <param name="newValue">The new boxed.</param>
    /// <param name="key">The property key of the boxed changed.</param>
    /// <param name="autoMethod">true if set method automatically by boxed parameters; otherwise, false.</param>
    public static void Invoke<T>(this ChangeEventHandler<T> handler, object sender, T oldValue, T newValue, string key = null, bool autoMethod = false)
    {
        if (handler == null) return;
        var args = new ChangeEventArgs<T>(oldValue, newValue, key, autoMethod);
        handler(sender, args);
    }

    /// <summary>
    /// Occurs the event handler.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old boxed.</param>
    /// <param name="newValue">The new boxed.</param>
    /// <param name="triggerType">The type identifier of the trigger.</param>
    /// <param name="key">The property key of the boxed changed.</param>
    public static void Invoke<T>(this ChangeEventHandler<T> handler, object sender, T oldValue, T newValue, Guid triggerType, string key = null)
    {
        if (handler == null) return;
        var args = new ChangeEventArgs<T>(oldValue, newValue, triggerType, key);
        handler(sender, args);
    }

    /// <summary>
    /// Resolves a singleton instance.
    /// </summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <param name="resolver">The singleton resolver.</param>
    /// <returns>An instance resolved.</returns>
    /// <exception cref="NotSupportedException">The type of instance was not support to resolve.</exception>
    public static T Resolve<T>(this ISingletonResolver resolver)
    {
        if (resolver == null) throw ArgumentNull(nameof(resolver));
        return resolver.Resolve<T>(null);
    }

    /// <summary>
    /// Resolves a singleton instance.
    /// </summary>
    /// <param name="resolver">The singleton resolver.</param>
    /// <param name="type">The type of instance.</param>
    /// <param name="key">The key.</param>
    /// <returns>An instance resolved.</returns>
    /// <exception cref="NotSupportedException">The type of instance was not support to resolve.</exception>
    public static object Resolve(this ISingletonResolver resolver, Type type, string key = null)
    {
        if (resolver == null) throw ArgumentNull(nameof(resolver));
        var resolverType = typeof(ISingletonResolver);
        var method = resolverType.GetMethod("Resolve", new[] { typeof(string) });
        var genericMethod = method.MakeGenericMethod(new [] { type });
        return genericMethod.Invoke(resolver, new object[] { key });
    }

    /// <summary>
    /// Resolves a singleton instance.
    /// </summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <param name="resolver">The singleton resolver.</param>
    /// <param name="result">An instance resolved.</param>
    /// <returns>true if resolve succeeded; otherwise, false.</returns>
    public static bool TryResolve<T>(this ISingletonResolver resolver, out T result)
    {
        if (resolver != null) return resolver.TryResolve(null, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tests if the given type is from a generic enumerable interface.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <param name="genericType">The generic type.</param>
    /// <returns>true if the type is from a generic enumerable interface.</returns>
    public static bool IsGenericEnumerable(Type type, out Type genericType)
    {
        try
        {
            var generic = type?.IsGenericType == true ? type.GetGenericTypeDefinition() : null;
            if (generic == null)
            {
                genericType = default;
                return false;
            }

            genericType = type.GetGenericArguments().FirstOrDefault();
            if (genericType == null) return false;
            var baseType = typeof(IEnumerable<>).MakeGenericType(new[] { genericType });
            return baseType.IsAssignableFrom(type);
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }

        genericType = default;
        return false;
    }

    /// <summary>
    /// Tests if the given type is from a generic enumerable interface.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <returns>true if the type is from a generic enumerable interface.</returns>
    public static bool IsGenericEnumerable(Type type)
        => IsGenericEnumerable(type, out _);

    /// <summary>
    /// Tests if the given type is a nullable boxed type.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <returns>true if the type is a nullable boxed type.</returns>
    public static bool IsNullableValueType(Type type)
        => type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Tests if the given type is a nullable boxed type.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <param name="value">The type of the boxed type.</param>
    /// <returns>true if the type is a nullable boxed type.</returns>
    public static bool IsNullableValueType(Type type, out Type value)
    {
        if (!IsNullableValueType(type))
        {
            value = default;
            return false;
        }

        try
        {
            value = type.GetGenericArguments().FirstOrDefault();
            return value != null;
        }
        catch (InvalidOperationException)
        {
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to create an instance of the specific type.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <param name="result">The instance result.</param>
    /// <returns>true if create succeeded; otherwise, false.</returns>
    public static bool TryCreateInstance<T>(out T result)
    {
        try
        {
            result = Activator.CreateInstance<T>();
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (TargetInvocationException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (TypeLoadException)
        {
        }
        catch (InvalidComObjectException)
        {
        }
        catch (ExternalException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to create an instance of the specific type.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <param name="baseType">The base type.</param>
    /// <param name="result">The instance result.</param>
    /// <returns>true if create succeeded; otherwise, false.</returns>
    public static bool TryCreateInstance<T>(Type baseType, out T result)
    {
        try
        {
            if (baseType != null && typeof(T).IsAssignableFrom(baseType) && Activator.CreateInstance(baseType) is T r)
            {
                result = r;
                return true;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (TargetInvocationException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (TypeLoadException)
        {
        }
        catch (InvalidComObjectException)
        {
        }
        catch (ExternalException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get a specific property boxed.
    /// </summary>
    /// <typeparam name="T">The type of the property boxed.</typeparam>
    /// <param name="obj">The target object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public static bool TryGetProperty<T>(object obj, string propertyName, out T result)
    {
        try
        {
            var prop = obj?.GetType()?.GetProperty(propertyName);
            if (prop != null && prop.CanRead && typeof(T).IsAssignableFrom(prop.PropertyType) && prop.GetValue(obj, null) is T r)
            {
                result = r;
                return true;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (AmbiguousMatchException)
        {
        }
        catch (TargetException)
        {
        }
        catch (TargetInvocationException)
        {
        }
        catch (TargetParameterCountException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ExternalException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get a specific property boxed.
    /// </summary>
    /// <typeparam name="T">The type of the property boxed.</typeparam>
    /// <param name="obj">The target object.</param>
    /// <param name="prop">The property info.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public static bool TryGetProperty<T>(object obj, PropertyInfo prop, out T result)
    {
        try
        {
            if (prop != null && prop.CanRead && typeof(T).IsAssignableFrom(prop.PropertyType) && prop.GetValue(obj, null) is T r)
            {
                result = r;
                return true;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (AmbiguousMatchException)
        {
        }
        catch (TargetException)
        {
        }
        catch (TargetInvocationException)
        {
        }
        catch (TargetParameterCountException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ExternalException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="obj">The object to resolve typed instance.</param>
    /// <param name="value">The value resolved.</param>
    /// <returns>true if has; otherwise, false.</returns>
    public static bool TryGet<T>(object obj, out T value)
    {
        if (TryGetForSimple(obj, out value)) return true;
        if (obj is BaseNestedParameter n) return n.ParameterIs(out value);
        if (obj is TypedNestedParameter t) return t.TryGet(out value);
        return false;
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>The description.</returns>
    public static Guid? GetGuid(MemberInfo memberInfo)
    {
        try
        {
            var attr = memberInfo.GetCustomAttributes<GuidAttribute>()?.FirstOrDefault();
            var str = attr?.Value;
            if (!string.IsNullOrWhiteSpace(str) && Guid.TryParse(str, out var id)) return id;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts a specific number to the hex string.
    /// </summary>
    /// <param name="number">The input number.</param>
    /// <returns>A hex string converted.</returns>
    public static string ToHexString(short number)
        => Maths.Numbers.ToPositionalNotationString(number, 16);

    /// <summary>
    /// Converts a specific number to the hex string.
    /// </summary>
    /// <param name="number">The input number.</param>
    /// <returns>A hex string converted.</returns>
    public static string ToHexString(int number)
        => Maths.Numbers.ToPositionalNotationString(number, 16);

    /// <summary>
    /// Converts a specific number to the hex string.
    /// </summary>
    /// <param name="number">The input number.</param>
    /// <returns>A hex string converted.</returns>
    public static string ToHexString(long number)
        => Maths.Numbers.ToPositionalNotationString(number, 16);

    /// <summary>
    /// Converts a specific number to the hex string.
    /// </summary>
    /// <param name="number">The input number.</param>
    /// <returns>A hex string converted.</returns>
    public static string ToHexString(float number)
        => Maths.Numbers.ToPositionalNotationString(number, 16);

    /// <summary>
    /// Converts a specific number to the hex string.
    /// </summary>
    /// <param name="number">The input number.</param>
    /// <returns>A hex string converted.</returns>
    /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
    public static string ToHexString(double number)
        => Maths.Numbers.ToPositionalNotationString(number, 16);

    /// <summary>
    /// Converts a specific GUID to the hex string.
    /// </summary>
    /// <param name="id">The input GUID.</param>
    /// <returns>A hex string converted.</returns>
    /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
    public static string ToHexString(Guid id)
        => id.ToString("N");

    /// <summary>
    /// Converts a specific byte collection to the hex string.
    /// </summary>
    /// <param name="bytes">The input byte collection.</param>
    /// <returns>A hex string converted.</returns>
    /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
    public static string ToHexString(IEnumerable<byte> bytes)
    {
        // Null checker.
        if (bytes == null) return null;

        // Create a new string builder to collect the bytes and create a string.
        var str = new StringBuilder();

        // Loop through each byte of the array and format each one as a hexadecimal string.
        foreach (var b in bytes)
        {
            str.Append(b.ToString("x2"));
        }

        // Return the hexadecimal string.
        return str.ToString();
    }

    /// <summary>
    /// Converts a specific hex string to byte collection.
    /// </summary>
    /// <param name="hex">The input hex string.</param>
    /// <returns>A byte collection.</returns>
    public static byte[] FromHexString(string hex)
    {
        if (hex == null) return Array.Empty<byte>();
        hex = hex.Replace(" ", string.Empty);
        var count = hex.Length / 2;
        var bytes = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var j = i * 2;
#if NET6_0_OR_GREATER
            var c = hex.AsSpan(j, 2);
#else
            var c = hex.Substring(j, 2);
#endif
            bytes[i] = int.TryParse(c, NumberStyles.HexNumber, null, out var val)
                ? (byte)val
                : throw CreateHexArgumentException(nameof(hex), j, c);
        }

        return bytes;
    }

    /// <summary>
    /// Converts a specific hex string to byte collection.
    /// </summary>
    /// <param name="hex">The input hex string.</param>
    /// <returns>A byte collection.</returns>
    public static IEnumerable<byte> ReadHexString(string hex)
    {
        if (hex == null) yield break;
        var count = hex.Length / 2;
        for (int i = 0; i < count; i++)
        {
            var j = i * 2;
#if NET6_0_OR_GREATER
            var c = hex.AsSpan(j, 2);
#else
            var c = hex.Substring(j, 2);
#endif
            yield return int.TryParse(c, NumberStyles.HexNumber, null, out var val)
                ? (byte)val
                : throw CreateHexArgumentException(nameof(hex), j, c);
        }
    }

    /// <summary>
    /// Converts a specific hex string to byte collection.
    /// </summary>
    /// <param name="hex">The input hex string.</param>
    /// <param name="bytes">A byte collection output.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryFromHexString(string hex, out byte[] bytes)
    {
        if (hex == null)
        {
            bytes = Array.Empty<byte>();
            return false;
        }

        hex = hex.Replace(" ", string.Empty);
        var count = hex.Length / 2;
        bytes = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var j = i * 2;
#if NET6_0_OR_GREATER
            var c = hex.AsSpan(j, 2);
#else
            var c = hex.Substring(j, 2);
#endif
            if (int.TryParse(c, NumberStyles.HexNumber, null, out var val))
                bytes[i] = (byte)val;
            else
                return false;
        }

        return true;
    }

    /// <summary>
    /// Converts a specific hex string to byte collection.
    /// </summary>
    /// <param name="hex">The input hex string.</param>
    /// <returns>A byte collection.</returns>
    public static byte[] TryFromHexString(string hex)
    {
        if (hex == null) return Array.Empty<byte>();
        hex = hex.Replace(" ", string.Empty);
        var count = hex.Length / 2;
        var bytes = new byte[count];
        for (int i = 0; i < count; i++)
        {
#if NET6_0_OR_GREATER
            bytes[i] = int.TryParse(hex.AsSpan(i * 2, 2), NumberStyles.HexNumber, null, out var val) ? (byte)val : byte.MinValue;
#else
            bytes[i] = int.TryParse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, null, out var val) ? (byte)val : byte.MinValue;
#endif
        }

        return bytes;
    }

    /// <summary>
    /// Converts a specific hex string to byte collection.
    /// </summary>
    /// <param name="hex">The input hex string.</param>
    /// <returns>A byte collection.</returns>
    public static IEnumerable<byte> TryReadHexString(string hex)
    {
        if (hex == null) yield break;
        var count = hex.Length / 2;
        for (int i = 0; i < count; i++)
        {
#if NET6_0_OR_GREATER
            yield return int.TryParse(hex.AsSpan(i * 2, 2), NumberStyles.HexNumber, null, out var val) ? (byte)val : byte.MinValue;
#else
            yield return int.TryParse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, null, out var val) ? (byte)val : byte.MinValue;
#endif
        }
    }

    internal static ArgumentNullException ArgumentNull(string paramName)
        => new(paramName, string.Concat(paramName, " should not be null."));

    internal static bool Equals<T>(T a, T b) where T : IEquatable<T>
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    internal static bool TryGetForSimple<T>(object obj, out T boxed)
    {
        if (obj is null)
        {
            if (typeof(T) == typeof(DBNull))
            {
                boxed = (T)(object)DBNull.Value;
                return true;
            }

            boxed = default;
            return false;
        }

        if (obj is T o)
        {
            boxed = o;
            return true;
        }

        try
        {
            if (obj is IObjectRef<T> r)
            {
                boxed = r.Value;
                return true;
            }

            if (obj is Lazy<T> l)
            {
                boxed = l.Value;
                return true;
            }

            if (obj is IObjectResolver<T> h)
            {
                boxed = h.GetInstance();
                return true;
            }

            if (obj is IJsonValueNode<T> j)
            {
                boxed = j.Value;
                return true;
            }
        }
        catch (MemberAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (KeyNotFoundException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (FormatException)
        {
        }

        boxed = default;
        return false;
    }

    internal static T ParseEnum<T>(string s) where T : struct
#if NETFRAMEWORK
        => (T)Enum.Parse(typeof(T), s);
#else
        => Enum.Parse<T>(s);
#endif

    internal static T ParseEnum<T>(string s, bool ignoreCase) where T : struct
#if NETFRAMEWORK
        => (T)Enum.Parse(typeof(T), s, ignoreCase);
#else
        => Enum.Parse<T>(s, ignoreCase);
#endif

    private static ArgumentException CreateHexArgumentException(string paramName, int index, ReadOnlySpan<char> c)
        => CreateHexArgumentException(paramName, index, c.ToString());

    private static ArgumentException CreateHexArgumentException(string paramName, int index, string c)
        => new(string.Concat(paramName, " should be a hex string."), paramName, new ArgumentOutOfRangeException(paramName, string.Concat("Index ", index, " (", c, ") is not a hex digit.")));
}
