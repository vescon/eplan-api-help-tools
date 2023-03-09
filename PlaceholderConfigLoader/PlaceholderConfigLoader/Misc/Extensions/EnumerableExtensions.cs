using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlaceholderConfigLoader.Extensions
{
    /// <summary>Extension methods for generic sequences.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if <paramref name="list" /> contains exactly the elements of the <paramref name="listToCompare" />. The order of the items is irrelevant.
        /// </summary>
        /// <param name="list">The source list.</param>
        /// <param name="listToCompare">The list which is compared to the source list.</param>
        /// <returns>Returns true, if the lists contain exactly the same items.</returns>
        public static bool ContainsExactly<T>(
            this IEnumerable<T> list, 
            IEnumerable<T> listToCompare)
        {
            return list.ToSet().SetEquals(listToCompare.ToSet());
        }

        /// <summary>
        /// Builds pairs of the sequence items of both sequences by internally using <see cref="M:System.Linq.Enumerable.Zip``3(System.Collections.Generic.IEnumerable{``0},System.Collections.Generic.IEnumerable{``1},System.Func{``0,``1,``2})" />.
        /// </summary>
        /// <param name="firstSource">The first source sequence.</param>
        /// <param name="secondSource">The second source sequence.</param>
        /// <returns>Returns a list of pairs of the <paramref name="firstSource" /> and <paramref name="secondSource" />. [[firstSource[0], secondSource[0]], [firstSource[1], secondSource[1]], ...]</returns>
        public static IEnumerable<Pair<TFirst, TSecond>> Zip<TFirst, TSecond>(
            this IEnumerable<TFirst> firstSource, 
            IEnumerable<TSecond> secondSource)
        {
            return firstSource.Zip(secondSource, (first, second) => new Pair<TFirst, TSecond>(first, second));
        }

        /// <summary>Converts the sequence to a Hashset.</summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns a <see cref="T:System.Collections.Generic.HashSet`1" /> using the default equality comparer for {T}.</returns>
        public static ISet<T> ToSet<T>(this IEnumerable<T> list)
        {
            return new HashSet<T>(list);
        }

        /// <summary>
        /// Gets <paramref name="list" /> or an empty sequence, if it's null.
        /// </summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns <paramref name="list" /> itself or an empty sequence, if it's null.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        /// <summary>Checks if the sequence is not empty.</summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns true, if it contains at least one element.</returns>
        public static bool NotEmpty<T>(this IEnumerable<T> list)
        {
            return list.Any();
        }

        /// <summary>Checks if the sequence is empty.</summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns true, if it does not contain any element.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }

        /// <summary>Checks if the sequence is null or empty.</summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns true, if the sequence is not or empty.</returns>
        public static bool IsEmptyOrNull<T>(this IEnumerable<T> list)
        {
            if (list != null)
                return list.IsEmpty();
            return true;
        }

        /// <summary>
        /// Concatenates the elements with the ToString() method and delimits them by <see cref="P:System.Environment.NewLine" />.
        /// </summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns the concatenated string of all entries in the sequence, delimited by new lines.</returns>
        public static string Concatenate<T>(this IEnumerable<T> list)
        {
            return list.Concatenate(Environment.NewLine);
        }

        /// <summary>
        /// Concatenates the elements with the ToString() method and delimits them by the given <paramref name="delimiter" />.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="delimiter">The delimiter which is used to separate one element from the next one.</param>
        /// <returns>Returns the concatenated string of all entries in the sequence, delimited by <paramref name="delimiter" />.</returns>
        public static string Concatenate<T>(this IEnumerable<T> source, string delimiter)
        {
            return string.Join(delimiter, source);
        }

        /// <summary>
        /// Shuffles all entries in the sequence. The original sequence is not changed.
        /// </summary>
        /// <param name="list">The source sequence.</param>
        /// <returns>Returns a new array with the shuffled entries.</returns>
        public static T[] Randomize<T>(this IEnumerable<T> list)
        {
            return list.Randomize(Environment.TickCount);
        }

        /// <summary>
        /// Shuffles all entries in the sequence. The original sequence is not changed.
        /// </summary>
        /// <param name="list">The source sequence.</param>
        /// <param name="seed">The seed value for the <see cref="T:System.Random" /> class.</param>
        /// <returns>Returns a new array with the shuffled entries.</returns>
        public static T[] Randomize<T>(this IEnumerable<T> list, int seed)
        {
            Random random = new Random(seed);
            T[] array = list.ToArray();
            for (int index1 = array.Length - 1; index1 > 0; --index1)
            {
                int index2 = random.Next(index1 + 1);
                Swap(ref array[index1], ref array[index2]);
            }
            return array;
        }

        /// <summary>
        /// Skips the last <paramref name="count" /> entries of the list.
        /// </summary>
        /// <param name="list">The source sequence.</param>
        /// <param name="count">The number of ignored elements at the end of the sequence.</param>
        /// <returns>Returns a new sequence, where the last <paramref name="count" /> entries are ignored.</returns>
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> list, int count)
        {
            Queue<T> buffer = new Queue<T>(count);
            foreach (T obj in list)
            {
                buffer.Enqueue(obj);
                if (buffer.Count > count)
                    yield return buffer.Dequeue();
            }
        }

        /// <summary>
        /// Gets the element with the minimum value of the returned <paramref name="selector" />.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">The selector, which returns the value, which is used for the comparison.</param>
        /// <returns>Returns the element with the minimum value of the returned <paramref name="selector" />.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Gets the element with the minimum value of the returned <paramref name="selector" />.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">The selector, which returns the value, which is used for the comparison.</param>
        /// <param name="comparer">Defines the used comparer.</param>
        /// <returns>Returns the element with the minimum value of the returned <paramref name="selector" />.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            TSource source1 = default (TSource);
            TKey y = default (TKey);
            bool flag = false;
            foreach (TSource source2 in source)
            {
                TKey x = selector(source2);
                if (!flag || comparer.Compare(x, y) < 0)
                {
                    source1 = source2;
                    y = x;
                    flag = true;
                }
            }
            return source1;
        }

        /// <summary>
        /// Checks if any item in the sequence <paramref name="source" /> is assignable to any type in <paramref name="types" />.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="types">The types which are checked.</param>
        /// <returns>Returns true if any type is assignable to at least one item in the sequence.</returns>
        public static bool AnyElementIsOfType<T>(this IEnumerable<T> source, params Type[] types)
        {
            return types.Any(t => source.Any(s => s.GetType().IsAssignableFrom(t)));
        }

        /// <summary>
        /// Applies the <paramref name="action" /> on each element in the sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action, which is executed for an element.</param>
        /// <returns>Returns a sequence over the elements in the input sequence.</returns>
        public static IEnumerable<T> OnEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            return source.OnEach(action, x => true);
        }

        /// <summary>
        /// Applies the <paramref name="action" /> on each element in the sequence, where the <paramref name="predicate" /> returns true.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action, which is executed for an element.</param>
        /// <param name="predicate">Only executes the <paramref name="action" /> for elements, where this predicate is true. If it is null, then <paramref name="action" /> is executed for each element.</param>
        /// <returns>Returns a sequence over all elements weather the <paramref name="predicate" /> returns true or false.</returns>
        public static IEnumerable<T> OnEach<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> predicate)
        {
            foreach (T obj in source.Where(predicate))
            {
                action(obj);
                yield return obj;
            }
        }

        /// <summary>Checks if to sequences contain the same elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="sequenceToCompare">The source sequence.</param>
        /// <returns>Returns true, if both sequences contain the same elements. The order is irrelevant.</returns>
        public static bool SetEqual<T>(this IEnumerable<T> source, IEnumerable<T> sequenceToCompare)
        {
            return new HashSet<T>(source).SetEquals(sequenceToCompare);
        }

        /// <summary>
        /// Applies the <paramref name="action" /> on each element in the sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action, which is executed for an element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
                action(obj);
        }

        /// <summary>
        /// Applies the <paramref name="action" /> on each element in the sequence. The action contains an additional parameter with the index.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action, including the index as parameter, which is executed for an element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int num = 0;
            foreach (T obj in source)
            {
                action(obj, num);
                ++num;
            }
        }

        /// <summary>
        /// Gets a new sequence with the elements which are exactly of the type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type for which the sequence is filtered.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Returns a new sequence which only contains elements of type <typeparamref name="T" /></returns>
        public static IEnumerable<T> OfExactType<T>(this IEnumerable source)
        {
            return source.OfType<T>().Where(TypeHelper.IsExactType);
        }

        /// <summary>Wraps the value in an enumerable.</summary>
        /// <param name="value">The value, which is wrapped by an enumerable.</param>
        /// <returns>Returns a generic sequence, which contains <paramref name="value" /> as the single element.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            yield return value;
        }

        /// <summary>Wraps the value in a list.</summary>
        /// <param name="value">The value, which is wrapped by the list.</param>
        /// <returns>Returns a generic list, which contains the <paramref name="value" /> as the single element.</returns>
        public static List<T> AsList<T>(this T value)
        {
            return new List<T> { value };
        }

        /// <summary>
        /// Groups the sequence into multiple sequences with the given <paramref name="batchSize" />.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="batchSize">The number of items in the sub sequences.</param>
        /// <returns>Returns a sequence of sequences with the given <paramref name="batchSize" />. The last sequence contains the remaining items.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (batchSize == 0)
            {
                yield return source;
            }
            else
            {
                List<T> batch = new List<T>(batchSize);
                foreach (T obj in source)
                {
                    batch.Add(obj);
                    if (batch.Count >= batchSize)
                    {
                        yield return batch;
                        batch = new List<T>(batchSize);
                    }
                }
                if (batch.Count > 0)
                    yield return batch;
            }
        }

        /// <summary>
        /// Flattens the hierarichal parent/children sequence by the <paramref name="childrenSelector" />.
        /// </summary>
        /// <param name="source">The hierarichal source sequence.</param>
        /// <param name="childrenSelector">The selector, which resolves the children of a <typeparam name="T" />.</param>
        /// <returns>Returns the recursively flattened sequence.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            foreach (T obj1 in source)
            {
                yield return obj1;
                foreach (T obj2 in childrenSelector(obj1).Flatten(childrenSelector))
                    yield return obj2;
            }
        }

        /// <summary>Gets the type of the generic sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>Returns the type object of the generic sequence.</returns>
        public static Type GetElementType(this IEnumerable source)
        {
            return source.GetType().GetInterfaces()
                .Where(type => type.IsGenericType)
                .Where(type => type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                .Select(type => type.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        /// <summary>
        /// Finds the top parents in list of elements in a parent/child hierarchy.
        /// </summary>
        /// <param name="source">An sequence of instances with a parent/child hierarchy.</param>
        /// <param name="rootSelector">This selector points to the parent of a single item the sequence.</param>
        /// <returns>Returns a sequence of the top parent elements in the hierarchy for each element in <paramref name="source" />.</returns>
        public static IEnumerable<T> Rooten<T>(this IEnumerable<T> source, Func<T, T> rootSelector) where T : class
        {
            return source.Select(item => GetRootRecursive(item, rootSelector));
        }

        /// <summary>
        /// Finds the top parent of an element in a parent/child hierarchy.
        /// </summary>
        /// <param name="source">An instance in the parent/child hierarchy.</param>
        /// <param name="rootSelector">This selector points to the parent of this instance.</param>
        /// <returns>Returns the top parent element in the hierarchy, selected by the <paramref name="rootSelector" />.</returns>
        public static T GetRootRecursive<T>(T source, Func<T, T> rootSelector) where T : class
        {
            T source1 = rootSelector(source);
            if (source1 == null)
                return source;
            return GetRootRecursive(source1, rootSelector);
        }

        /// <summary>
        /// Wraps every element in the sequence in parenthesis. E.g. "Element" results in "(Element)".
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>Returns a sequence of parenthesized strings based on <paramref name="source" />.</returns>
        public static IEnumerable<string> Parenthesize<T>(this IEnumerable<T> source)
        {
            return source.Parenthesize("(", ")");
        }

        /// <summary>
        /// Wraps every element in the sequence in parenthesis. E.g. "Element" results in "'Element'".
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="parenthesisLeftAndRight">Defines the left and right parenthesis of one element of <paramref name="source" />.</param>
        /// <returns>Returns a sequence of parenthesized strings based on <paramref name="source" />.</returns>
        public static IEnumerable<string> Parenthesize<T>(this IEnumerable<T> source, string parenthesisLeftAndRight)
        {
            return source.Parenthesize(parenthesisLeftAndRight, parenthesisLeftAndRight);
        }

        /// <summary>
        /// Wraps every element in the sequence in parenthesis. The parenthesis can be defined.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="parenthesisLeft">Defines the left parenthesis of one element of <paramref name="source" />.</param>
        /// <param name="parenthesisRight">Defines the right parenthesis of one element of <paramref name="source" />.</param>
        /// <returns>Returns a sequence of parenthesized strings based on <paramref name="source" />.</returns>
        public static IEnumerable<string> Parenthesize<T>(this IEnumerable<T> source, string parenthesisLeft, string parenthesisRight)
        {
            return source.Select(entry => parenthesisLeft + entry.ToString() + parenthesisRight);
        }

        /// <summary>Filter all values of the sequence which are not null</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>Returns a sequence of type <typeparam name="T"></typeparam> with the elements of <paramref name="source" /> excluding all elements which don't have a value.</returns>
        public static IEnumerable<T> WithoutNulls<T>(this IEnumerable<T?> source) where T : struct
        {
            return source.Where(o => o.HasValue).Select(o => o.GetValueOrDefault());
        }

        /// <summary>
        /// Finds the index of the first found item in the sequence or -1.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">The filter for the sequence.</param>
        /// <returns>Returns the index of the first found element on which <paramref name="predicate" /> matches. If no element could be found an exception is thrown.</returns>
        /// <exception cref="T:System.InvalidOperationException"></exception>
        public static int FirstIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int num = source.FirstIndexOrDefault(predicate);
            if (num == -1)
                throw new InvalidOperationException("No element found!");
            return num;
        }

        /// <summary>
        /// Finds the index of the first found item in the sequence or -1.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">The filter for the sequence.</param>
        /// <returns>Returns the index of the first found element on which <paramref name="predicate" /> matches. If no element could be found, then -1 is returned.</returns>
        public static int FirstIndexOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int num = 0;
            foreach (T obj in source)
            {
                if (predicate(obj))
                    return num;
                ++num;
            }
            return -1;
        }

        private static void Swap<T>(ref T element1, ref T element2)
        {
            T obj = element2;
            element2 = element1;
            element1 = obj;
        }

        private static class TypeHelper
        {
            public static bool IsExactType<T>(T item)
            {
                return !IsDerivedClass(item);
            }

            private static bool IsDerivedClass<T>(T item)
            {
                return !item.GetType().IsAssignableFrom(typeof (T));
            }
        }
    }
}