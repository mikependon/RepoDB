using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// An extension class for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Return the items of the enumerable on the defined range.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <param name="startIndex">The starting index in which to start the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns>The items within the range of the enumerable.</returns>
        public static IEnumerable<T> Range<T>(this IEnumerable<T> value,
            int startIndex,
            int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Value");
            }
            for (var i = startIndex; i < (startIndex + length); i++)
            {
                yield return value.ElementAt(i);
            }
        }

        /// <summary>
        /// Split the enumerable into multiple enumerables.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <param name="sizePerSplit">The sizes of the items per split.</param>
        /// <returns>An enumerable of enumerables.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> value,
            int sizePerSplit)
        {
            var itemCount = value.Count();
            if (itemCount <= sizePerSplit)
            {
                yield return value;
            }
            else
            {
                var batchCount = Convert.ToInt32(itemCount / sizePerSplit) + (itemCount % sizePerSplit);
                for (var i = 0; i < batchCount; i++)
                {
                    yield return Enumerable.Where(value, (item, index) =>
                    {
                        return index >= (sizePerSplit * i) &&
                            index < (sizePerSplit * i) + sizePerSplit;
                    });
                }
            }
        }

        /// <summary>
        /// Converts the <see cref="IEnumerable{T}"/> object into a <see cref="IList{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static List<T> AsList<T>(this IEnumerable<T> value)
        {
            return value is List<T> ? (List<T>)value : value?.ToList();
        }

        /// <summary>
        /// Converts the <see cref="IEnumerable{T}"/> object into an array of objects.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static T[] AsArray<T>(this IEnumerable<T> value)
        {
            return value is T[]? (T[])value : value.ToArray();
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="eachAction">iterative action</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> eachAction)
        {
            foreach (var item in collection)
            {
                eachAction?.Invoke(item);
            }
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="eachAction">iterative action</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> eachAction)
        {
            var index = 0;
            foreach (var item in collection)
            {
                eachAction?.Invoke(item, index++);
            }
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <typeparam name="TU">result type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="eachFunc">select function</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<TU> ForEach<T, TU>(this IEnumerable<T> collection, Func<T, TU> eachFunc)
        {
            return collection.Select(eachFunc.Invoke).ToList();
        }
    }
}
