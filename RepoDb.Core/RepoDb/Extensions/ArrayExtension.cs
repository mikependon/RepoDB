using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="System.Array"/> object.
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Converts an <see cref="Array"/> object into an enumerable of objects.
        /// </summary>
        /// <param name="array">The array to be converted.</param>
        /// <returns>An enumerable of objects.</returns>
        public static IEnumerable<object> AsEnumerable(this Array array) =>
            AsEnumerable<object>(array);

        /// <summary>
        /// Converts an <see cref="Array"/> object into an enumerable of objects.
        /// </summary>
        /// <typeparam name="T">The type of the object to be filtered.</typeparam>
        /// <param name="array">The array to be converted.</param>
        /// <returns>An enumerable of objects.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this Array array)
        {
            if (array != null)
            {
                foreach (var value in array)
                {
                    if (value is T t)
                    {
                        yield return t;
                    }
                }
            }
        }
    }
}