using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Convert given dictionary to object by mapping dictionary keys to object properties
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="source">the dictionary to convert</param>
        /// <returns>new object of T contains data mapped from dictionary</returns>
        /// <exception cref="ArgumentException">If the given dictionary is empty</exception>
        /// <exception cref="ArgumentNullException">If the given dictionary is null</exception>
        public static T ToObject<T>([NotNull] this IDictionary<string, object> source) where T : class, new()
        {
            Guard.Against.NullOrEmptyCollection(source, nameof(source));

            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        /// <summary>
        /// Convert given dictionary to querystring
        /// </summary>
        /// <param name="source">the dictionary to convert</param>
        /// <returns>the querystring</returns>
        /// <exception cref="ArgumentException">If the given dictionary is empty</exception>
        /// <exception cref="ArgumentNullException">If the given dictionary is null</exception>
        public static string ToQueryString([NotNull] this IDictionary<string, object> source)
        {
            Guard.Against.NullOrEmptyCollection(source, nameof(source));

            return string.Join("&", source.Select(x => $"{x.Key}={ConvertToString(x.Value)}"));
        }

        private static string ConvertToString([NotNull] object value)
        {
            if (value is null)
                return null;

            if (value is DateTime time)
                return time.ToString(CultureInfo.InvariantCulture);

            return value.ToString();
        }


        /// <summary>
        /// This method is used to try to get a value in a dictionary if it does exists.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="source">The collection object</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value of the key (or default value if key not exists)</param>
        /// <returns>True if key does exists in the dictionary</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key is null</exception>
        internal static bool TryGetValue<T>(this IDictionary<string, object> source, [NotNull] string key, out T value)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));

            if (source.TryGetValue(key, out object valueObj) && valueObj is T val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key is null</exception>
        public static TValue GetOrDefault<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> source, [NotNull] TKey key)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));

            return source.TryGetValue(key, out TValue obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key is null</exception>
        public static TValue GetOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> source, [NotNull] TKey key)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));

            return source.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key is null</exception>
        public static TValue GetOrDefault<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> source, [NotNull] TKey key)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));

            return source.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key is null</exception>
        public static TValue GetOrDefault<TKey, TValue>([NotNull] this ConcurrentDictionary<TKey, TValue> source, [NotNull] TKey key)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));

            return source.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key or factory is null</exception>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> source, [NotNull] TKey key, [NotNull] Func<TKey, TValue> factory)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));
            Guard.Against.Null(key, nameof(key));

            if (source.TryGetValue(key, out TValue obj))
                return obj;

            return source[key] = factory(key);
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key or factory is null</exception>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> source, [NotNull] TKey key, [NotNull] Func<TValue> factory)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));
            Guard.Against.Null(key, nameof(key));

            return source.GetOrAdd(key, k => factory());
        }

        /// <summary>
        /// Gets a value from the concurrent dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="source">Concurrent dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        /// <exception cref="ArgumentNullException">If source dictionary or key or factory is null</exception>
        public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> source, TKey key, Func<TValue> factory)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(key, nameof(key));
            Guard.Against.Null(key, nameof(key));

            return source.GetOrAdd(key, k => factory());
        }
    }
}