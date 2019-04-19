using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="ClassProperty"/>.
    /// </summary>
    internal static class ClassPropertyExtension
    {
        /// <summary>
        /// Convert the <see cref="ClassProperty"/> into a <see cref="Field"/> objects.
        /// </summary>
        /// <param name="property">The current instance of <see cref="ClassProperty"/>.</param>
        /// <returns>An instance of <see cref="string"/> object.</returns>
        public static Field AsField(this ClassProperty property)
        {
            return new Field(property.PropertyInfo.AsField());
        }

        /// <summary>
        /// Converts the list of <see cref="ClassProperty"/> into a a list of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="properties">The current instance of <see cref="ClassProperty"/>.</param>
        /// <returns>A list of <see cref="string"/> objects.</returns>
        public static IEnumerable<Field> AsFields(this IEnumerable<ClassProperty> properties)
        {
            foreach (var property in properties)
            {
                yield return AsField(property);
            }
        }
    }
}
