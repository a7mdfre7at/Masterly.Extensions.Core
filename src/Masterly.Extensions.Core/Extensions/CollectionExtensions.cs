using System.Linq;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>([CanBeNull] this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        /// <exception cref="ArgumentNullException">If the given source collection is null</exception>
        /// <exception cref="ArgumentNullException">If the given items collection is null</exception>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] T item)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(item, nameof(item));

            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }

        /// <summary>
        /// Adds items to the collection which are not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="items">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns the added items.</returns>
        /// <exception cref="ArgumentNullException">If the given collection is null</exception>
        /// <exception cref="ArgumentNullException">If the given items collection is null</exception>
        public static IEnumerable<T> AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] IEnumerable<T> items)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(items, nameof(items));

            var addedItems = new List<T>();

            foreach (T item in items)
            {
                if (source.Contains(item))
                    continue;

                source.Add(item);
                addedItems.Add(item);
            }

            return addedItems;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to decide if the item is already in the collection</param>
        /// <param name="itemFactory">A factory that returns the item</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        /// <exception cref="ArgumentNullException">If the collection, predecate or itemFactory is null</exception>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] Func<T, bool> predicate, [NotNull] Func<T> itemFactory)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(predicate, nameof(predicate));
            Guard.Against.Null(itemFactory, nameof(itemFactory));

            if (source.Any(predicate))
                return false;

            source.Add(itemFactory());
            return true;
        }

        /// <summary>
        /// Removes all items from the collection those satisfy the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to remove the items</param>
        /// <returns>List of removed items</returns>
        /// <exception cref="ArgumentNullException">If the given collection is null</exception>
        /// <exception cref="ArgumentNullException">If the given predicate is null</exception>
        public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, [NotNull] Func<T, bool> predicate)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(predicate, nameof(predicate));

            List<T> items = source.Where(predicate).ToList();

            foreach (T item in items)
                source.Remove(item);

            return items;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="items">Items to be removed from the list</param>
        /// <exception cref="ArgumentNullException">If source collection is null</exception>
        /// <exception cref="ArgumentNullException">If items collection is null</exception>
        public static void RemoveAll<T>([NotNull] this ICollection<T> source, [NotNull] IEnumerable<T> items)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(items, nameof(items));

            foreach (T item in items)
                source.Remove(item);
        }
    }
}