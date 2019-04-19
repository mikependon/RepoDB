using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtension
    {
        /// <summary>
        /// Converts all properties of the type into an array of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="string"/> objects.</returns>
        public static IEnumerable<Field> AsFields(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                yield return new Field(property.AsField());
            }
        }
    }
}
