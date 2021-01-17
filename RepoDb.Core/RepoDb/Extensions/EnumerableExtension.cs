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
                    array[i] = list.Where((_, index) =>
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
        /// Returns the items of type <typeparamref name="TargetType"/> from the <see cref="IEnumerable{T}"/> object into a target <see cref="IEnumerable{T}"/> object.
        /// </summary>
        /// <typeparam name="SourceType">The source type.</typeparam>
        /// <typeparam name="TargetType">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> object in which the items are of type <typeparamref name="TargetType"/>.</returns>
        [Obsolete("Use the 'WithType<T>' method instead.")]
        public static IEnumerable<TargetType> OfTargetType<SourceType, TargetType>(this IEnumerable<SourceType> value) =>
            value is IEnumerable<TargetType> enumerable ? enumerable : value.OfType<TargetType>();

        /// <summary>
        /// Checks whether the instance of <see cref="System.Collections.IEnumerable"/> is of type <see cref="IEnumerable{T}"/>, then casts it, otherwise, 
        /// returns the instance of <see cref="IEnumerable{T}"/> with the specified items. The items that are not of type <typeparamref name="T"/> will be
        /// eliminated from the result. This method is using the underlying method <see cref="Enumerable.OfType{TResult}(System.Collections.IEnumerable)"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> object in which the items are of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> WithType<T>(this System.Collections.IEnumerable value) =>
            value is IEnumerable<T> enumerable ? enumerable : value.OfType<T>();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is of type <see cref="List{T}"/>, then casts it, otherwise, converts it.
        /// This method is using the underlying method <see cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/> method.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="IList{T}"/> object.</returns>
        public static List<T> AsList<T>(this IEnumerable<T> value) =>
            value is List<T> list ? list : value?.ToList();

        /// <summary>
        /// Checks whether the instance of <see cref="IEnumerable{T}"/> is an array of <typeparamref name="T"/>, then casts it, otherwise, converts it.
        /// This method is using the underlying method <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> method.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The actual enumerable instance.</param>
        /// <returns>The converted <see cref="Array"/> object.</returns>
        public static T[] AsArray<T>(this IEnumerable<T> value) =>
            value is T[] array ? array : value?.ToArray();
    }
}
