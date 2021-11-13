using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace System
{
    public static class ObjectExtensions
    {
        public static bool Is<T>(this object obj) => obj is T;

        public static bool Is(this object obj, Type type)
            => obj != null && obj.GetType() == type;

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
            => source.GetType().GetProperties(bindingAttr).Where(x => !x.GetIndexParameters().Any()).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        public static string ToQueryString(this object source, string separator = "&")
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(separator, nameof(separator));


            // Get all properties on the object
            var properties = source.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(source, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(source, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => x.Value.GetType().IsComplex() ? $"{x.Key}.{ToQueryString(x.Value, "&").Replace("&", $"&{x.Key}.")}" : string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

        /// <summary>
        /// Trim all String properties of the given object
        /// </summary>
        /// <param name="source"></param>
        public static void TrimStringProperties(this object source)
        {
            Guard.Against.Null(source, nameof(source));

            var stringProperties = source.GetType().GetProperties()
                              .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(source, null);
                if (currentValue != null)
                    stringProperty.SetValue(source, currentValue.Trim(), null);
            }
        }

        public static bool IsNumber(this object value)
        {
            Guard.Against.Null(value, nameof(value));

            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static string ToJsonString(this object obj) => JsonConvert.SerializeObject(obj);


        /// <summary>
        /// Used to simplify and beautify casting an object to a type.
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj) where T : class
        {
            Guard.Against.Null(obj, nameof(obj));

            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,Type)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj) where T : struct
        {
            Guard.Against.Null(obj, nameof(obj));

            if (typeof(T) == typeof(Guid))
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }


        public static object To(this object obj, Type type)
        {
            Guard.Against.Null(obj, nameof(obj));

            if (type == typeof(Guid))
                return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(obj.ToString());

            return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            Guard.Against.Null(item, nameof(item));
            Guard.Against.Null(list, nameof(list));

            return list.Contains(item);
        }

        /// <summary>
        /// Check if an item is in the given enumerable.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="items">Items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, IEnumerable<T> items)
        {
            Guard.Against.Null(item, nameof(item));
            Guard.Against.Null(items, nameof(items));

            return items.Contains(item);
        }

        /// <summary>
        /// Can be used to conditionally perform a function
        /// on an object and return the modified or the original object.
        /// It is useful for chained calls.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="condition">A condition</param>
        /// <param name="func">A function that is executed only if the condition is <code>true</code></param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Returns the modified object (by the <paramref name="func"/> if the <paramref name="condition"/> is <code>true</code>)
        /// or the original object if the <paramref name="condition"/> is <code>false</code>
        /// </returns>
        public static T If<T>(this T obj, bool condition, Func<T, T> func)
        {
            Guard.Against.Null(obj, nameof(obj));
            Guard.Against.Null(func, nameof(func));

            if (condition)
                return func(obj);

            return obj;
        }

        /// <summary>
        /// Can be used to conditionally perform an action
        /// on an object and return the original object.
        /// It is useful for chained calls on the object.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="condition">A condition</param>
        /// <param name="action">An action that is executed only if the condition is <code>true</code></param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Returns the original object.
        /// </returns>
        public static T If<T>(this T obj, bool condition, Action<T> action)
        {
            Guard.Against.Null(obj, nameof(obj));
            Guard.Against.Null(action, nameof(action));

            if (condition)
                action(obj);

            return obj;
        }
    }
}