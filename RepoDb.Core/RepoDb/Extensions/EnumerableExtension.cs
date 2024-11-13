using System;
using System.Collections;
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
            var count = value?.Count() ?? 0;
            if (sizePerSplit == 0 || count <= sizePerSplit)
            {
                return new[] { value };
            }
#if !NET
            else
            {
                var batchCount = Convert.ToInt32(count / sizePerSplit) + ((count % sizePerSplit) != 0 ? 1 : 0);
                var array = new IEnumerable<T>[batchCount];
                for (var i = 0; i < batchCount; i++)
                {
                    array[i] = value.Where((_, index) =>
                        {
                            return index >= (sizePerSplit * i) &&
                                index < (sizePerSplit * i) + sizePerSplit;
                        })
                        .AsList();
                }
                return array;
            }
#else
            return value.Chunk(sizePerSplit);
#endif
        }

        /// <summary>
        /// Returns the items of type <typeparamref name="TargetType"/> from the <see cref="IEnumerable{T}"/> object into a target <see cref="IEnumerable{T}"/> object.
        /// </summary>
        /// <typeparam name="SourceType">The source type.</typeparam>
        /// <typeparam name="TargetType">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> object in which the items are of type <typeparamref name="TargetType"/>.</returns>
        [Obsolete("Use the 'WithType<T>' method instead.")]
        public static IEnumerable<TargetType> OfTargetType<SourceType, TargetType>(this IEnumerable<SourceType> value) =>
            value as IEnumerable<TargetType> ?? value.OfType<TargetType>();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable"/> is of type <see cref="IEnumerable{T}"/>, then casts it, otherwise, 
        /// returns the instance of <see cref="IEnumerable{T}"/> with the specified items. The items that are not of type <typeparamref name="T"/> will be
        /// eliminated from the result. This method is using the underlying method <see cref="Enumerable.OfType{TResult}(IEnumerable)"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> object in which the items are of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> WithType<T>(this IEnumerable value) =>
            value as IEnumerable<T> ?? value.OfType<T>();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is of type <see cref="List{T}"/>, then casts it, otherwise, converts it.
        /// This method is using the underlying method <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> method.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static List<T> AsList<T>(this IEnumerable<T> value) =>
            value as List<T> ?? value?.ToList();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is an array of <typeparamref name="T"/>, then casts it, otherwise, converts it.
        /// This method is using the underlying method <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> method.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="Array"/> object.</returns>
        public static T[] AsArray<T>(this IEnumerable<T> value) =>
            value as T[] ?? value?.ToArray();

        /// <summary>
        /// Gets a value indicating whether the current collection is null or empty.
        /// </summary>
        /// <param name="value">The target type.</param>
        /// <typeparam name="T">The actual enumerable instance.</typeparam>
        /// <returns>A value indicating whether the collection is null or empty.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => !value?.Any() ?? true;

#if !NET
        /// <summary>
        /// CCreates a new <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The actual enumerable instance.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>The created <see cref="HashSet{T}"/> object.</returns>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) =>
            new(source, comparer);

        /// <summary>
        /// Creates a new <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements=.</typeparam>
        /// <param name="source">The actual enumerable instance.</param>
        /// <returns>The created <see cref="HashSet{T}"/> object.</returns>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new(source);
#endif
    }
}
