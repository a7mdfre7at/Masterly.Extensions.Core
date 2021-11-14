using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace System.Collections.Generic
{
    public static class EnumerableExtenstions
    {
        #region Recursive Join
        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, IEnumerable<TResult>, TResult> resultSelector)
        {
            return RecursiveJoin(source, parentKeySelector, childKeySelector,
               resultSelector, Comparer<TKey>.Default);
        }

        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, int, IEnumerable<TResult>, TResult> resultSelector)
        {
            return RecursiveJoin(source, parentKeySelector, childKeySelector,
               (TSource element, int depth, int index, IEnumerable<TResult> children)
                  => resultSelector(element, index, children));
        }

        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, IEnumerable<TResult>, TResult> resultSelector,
            IComparer<TKey> comparer)
        {
            return RecursiveJoin(source, parentKeySelector, childKeySelector,
               (TSource element, int depth, int index, IEnumerable<TResult> children)
                  => resultSelector(element, children), comparer);
        }

        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, int, IEnumerable<TResult>, TResult> resultSelector,
            IComparer<TKey> comparer)
        {
            return RecursiveJoin(source, parentKeySelector, childKeySelector,
               (TSource element, int depth, int index, IEnumerable<TResult> children)
                  => resultSelector(element, index, children), comparer);
        }

        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, int, int, IEnumerable<TResult>, TResult> resultSelector)
        {
            return RecursiveJoin(source, parentKeySelector, childKeySelector,
               resultSelector, Comparer<TKey>.Default);
        }

        public static IEnumerable<TResult> RecursiveJoin<TSource, TKey, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TKey> parentKeySelector,
            Func<TSource, TKey> childKeySelector,
            Func<TSource, int, int, IEnumerable<TResult>, TResult> resultSelector,
            IComparer<TKey> comparer)
        {
            // prevent source being enumerated more than once per RecursiveJoin call
            source = new LinkedList<TSource>(source);

            // fast binary search lookup
            var parents = new SortedDictionary<TKey, TSource>(comparer);
            var children = new SortedDictionary<TKey, LinkedList<TSource>>(comparer);

            foreach (TSource element in source)
            {
                parents[parentKeySelector(element)] = element;


                TKey childKey = childKeySelector(element);

                if (!children.TryGetValue(childKey, out LinkedList<TSource> list))
                {
                    children[childKey] = list = new LinkedList<TSource>();
                }

                list.AddLast(element);
            }

            // initialize to null otherwise compiler complains at single line assignment
            IEnumerable<TResult> childSelector(TSource parent, int depth)
            {
                if (children.TryGetValue(parentKeySelector(parent), out LinkedList<TSource> innerChildren))
                {
                    return innerChildren.Select((child, index)
                       => resultSelector(child, index, depth, childSelector(child, depth + 1)));
                }

                return Enumerable.Empty<TResult>();
            }

            return source.Where(element => !parents.ContainsKey(childKeySelector(element)))
               .Select((element, index) => resultSelector(element, index, 0, childSelector(element, 1)));
        }
        #endregion


        #region Recursive Select
        public static IEnumerable<TSource> RecursiveSelect<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector)
        {
            return RecursiveSelect(source, childSelector, element => element);
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, (element, index, depth) => selector(element));
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, (element, index, depth) => selector(element, index));
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, int, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, selector, 0);
        }

        private static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, int, TResult> selector, int depth)
        {
            return source.SelectMany((element, index) => Enumerable.Repeat(selector(element, index, depth), 1)
               .Concat(RecursiveSelect(childSelector(element) ?? Enumerable.Empty<TSource>(),
                  childSelector, selector, depth + 1)));
        }
        #endregion

        /// <summary>
        /// Produces the set union of sequences and single item by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A System.Collections.Generic.IEnumerable`1 whose distinct elements from the first set for the union.</param>
        /// <param name="item">An item for union</param>
        /// <returns>An System.Collections.Generic.IEnumerable`1 that contains the elements from both inputs excluding duplicates.</returns>
        /// <exception cref="ArgumentNullException">source or item is null</exception>
        public static IEnumerable<TSource> Union<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] TSource item)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(item, nameof(item));

            return source.Union(Enumerable.Repeat(item, 1));
        }

        /// <summary>
        /// Check if sequesnce is empty or not.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A System.Collections.Generic.IEnumerable to check</param>
        /// <returns>True if empty, otherwise false</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        public static bool IsEmpty<TSource>([NotNull] this IEnumerable<TSource> source)
        {
            Guard.Against.Null(source, nameof(source));

            return !source.Any();
        }

        /// <summary>
        /// Split array into mini arrays based on given size
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A given array to split</param>
        /// <param name="size">Max size of mini arrays</param>
        /// <returns>Set of sequences have the same size</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        public static IEnumerable<IEnumerable<TSource>> Split<TSource>([NotNull] this TSource[] source, int size)
        {
            Guard.Against.Null(source, nameof(source));

            for (int i = 0; i < (float)source.Length / size; i++)
                yield return source.Skip(i * size).Take(size);
        }


        /// <summary>
        /// Check if source sequence contains any item from target
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A given sequence to match</param>
        /// <param name="target">A given array to match with</param>
        /// <returns>True if there is match between any items in both sequences, otherwise false</returns>
        /// <exception cref="ArgumentNullException">source or target is null</exception>
        public static bool MatchAny<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] params TSource[] target)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(target, nameof(target));

            return source.Any(s => target.Any(t => t.Equals(s)));
        }

        /// <summary>
        /// Check if all source sequence items matchs all items from target sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A given sequence to match</param>
        /// <param name="target">A given array to match with</param>
        /// <returns>True if there is match between all items in both sequences, otherwise false</returns>
        /// <exception cref="ArgumentNullException">source or target is null</exception>
        public static bool MatchAll<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] params TSource[] target)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(source, nameof(source));

            if (source.Count() != target.Length)
                return false;

            return source.AllIn(target);
        }

        /// <summary>
        /// Check if all source sequence items exists in target sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A given sequence to match</param>
        /// <param name="target">A given array to match with</param>
        /// <returns>True if all source items exists in target, otherwise false</returns>
        /// <exception cref="ArgumentNullException">source or target is null</exception>
        public static bool AllIn<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] params TSource[] target)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(target, nameof(target));

            return source.All(s => target.Any(t => t.Equals(s)));
        }

        /// <summary>
        /// Check if all target sequence items exists in source sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="source">A given sequence to match</param>
        /// <param name="target">A given array to match with</param>
        /// <returns>True if all target items exists, otherwise false</returns>
        /// <exception cref="ArgumentNullException">source or target is null</exception>
        public static bool ContainsAll<T>([NotNull] this IEnumerable<T> source, params T[] target)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(target, nameof(target));

            return target.AllIn(source.ToArray());
        }

        /// <summary>
        /// Check if all target sequence items exists in source sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <param name="source">A given sequence to fill it in html table</param>
        /// <returns>HTML table string</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        public static string ToHtmlTable<TSource>([NotNull] this IEnumerable<TSource> source)
        {
            Guard.Against.Null(source, nameof(source));

            Type type = typeof(TSource);
            PropertyInfo[] props = type.GetProperties();
            var html = new StringBuilder("<table>");

            //Header
            html.Append("<thead><tr>");

            foreach (PropertyInfo p in props)
                html.Append($"<th>{p.Name}</th>");

            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");

            foreach (TSource item in source)
            {
                html.Append("<tr>");
                props.Select(s => s.GetValue(item)).ToList().ForEach(p =>
                {
                    html.Append($"<td>{p}</td>");
                });
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");

            return html.ToString();
        }
    }
}