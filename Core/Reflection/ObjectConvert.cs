using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Trivial.Reflection
{
    /// <summary>
    /// The type utility.
    /// </summary>
    public static class ObjectConvert
    {
        /// <summary>
        /// Converts the given object to a specific strtypeuct.
        /// </summary>
        /// <param name="type">The type of object to return.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object converted.</returns>
        /// <exception cref="InvalidCastException">Failed to convert.</exception>
        /// <exception cref="FormatException">value is not in a format recognized by conversion type.</exception>
        /// <exception cref="ArgumentNullException">type or value was null.</exception>
        /// <exception cref="ArgumentException">value was not the one in the type or content format supported.</exception>
        /// <exception cref="OverflowException">value was outside the range of the underlying type of the specific type to convert.</exception>
        public static object Invoke(Type type, object value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), "type should not be null.");
            if (type == typeof(string))
            {
                if (value is Stream streamValue)
                {
                    using (var reader = new StreamReader(streamValue))
                    {
                        return reader.ReadToEnd();
                    }
                }

                return Convert.ToString(value);
            }

            if (value == null)
            {
                if (type.IsClass || IsNullableValueType(type)) return null;
                throw new ArgumentNullException(nameof(value), "value should not be null.");
            }

            var objectType = value.GetType();
            if (objectType.IsValueType || objectType.IsEnum) return Convert.ChangeType(value, type);
            if (value is string str)
            {
                if (type == typeof(string)) return str;
                if (type.IsEnum)
                {
                    return Enum.Parse(type, str);
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
        /// <param name="value">The value to convert.</param>
        /// <returns>An object converted.</returns>
        /// <exception cref="InvalidCastException">Failed to convert.</exception>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentException">value is not the one in the type or content format supported.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of the specific type to convert.</exception>
        public static T Invoke<T>(object value)
        {
            return (T)Invoke(typeof(T), value);
        }

        /// <summary>
        /// Tries to convert the given object to a specific struct.
        /// </summary>
        /// <typeparam name="T">The type of the value type instance to return.</typeparam>
        /// <param name="value">The value to convert.</param>
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
        /// <param name="value">The value to convert.</param>
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
        /// <typeparam name="T">The type of the value type instance to return.</typeparam>
        /// <param name="value">The value to convert.</param>
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
        /// <typeparam name="T">The type of the value type instance to return.</typeparam>
        /// <param name="value">The value to convert.</param>
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

        private static bool IsNullableValueType(Type type)
        {
            return type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
