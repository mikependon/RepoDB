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
        /// Split the enumerable into multiple enumerables.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <param name="sizePerSplit">The sizes of the items per split.</param>
        /// <returns>An enumerable of enumerables.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> value,
            int sizePerSplit)
        {
            var list = value?.AsList();
            if (list.Count <= sizePerSplit)
            {
                return new[] { value };
            }
            else
            {
                var batchCount = Convert.ToInt32(list.Count / sizePerSplit) + ((list.Count % sizePerSplit) != 0 ? 1 : 0);
                var array = new IEnumerable<T>[batchCount];
                for (var i = 0; i < batchCount; i++)
                {
                    array[i] = list.Where((item, index) =>
                        {
                            return index >= (sizePerSplit * i) &&
                                index < (sizePerSplit * i) + sizePerSplit;
                        })
                        .AsList();
                }
                return array;
            }
        }

        /// <summary>
        /// Converts the <see cref="IEnumerable{T}"/> object into a target <see cref="IEnumerable{T}"/> object via <see cref="Enumerable.OfType{TResult}(System.Collections.IEnumerable)"/> method.
        /// </summary>
        /// <typeparam name="SourceType">The sorrce type.</typeparam>
        /// <typeparam name="TargetType">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> object.</returns>
        public static IEnumerable<TargetType> OfTargetType<SourceType, TargetType>(this IEnumerable<SourceType> value) =>
            value is IEnumerable<TargetType> ? (IEnumerable<TargetType>)value : value.OfType<TargetType>();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is of type <see cref="List{T}"/>, then casts it, otherwise, converts it.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static List<T> AsList<T>(this IEnumerable<T> value) =>
            value is List<T> ? (List<T>)value : value?.ToList();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is an array of <typeparamref name="T"/>, then casts it, otherwise, converts it.
        /// </summary>
        /// <typeparam name="T">The target dynamic type of the enumerable.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static T[] AsArray<T>(this IEnumerable<T> value) =>
            value is T[]? (T[])value : value.ToArray();
    }
}
