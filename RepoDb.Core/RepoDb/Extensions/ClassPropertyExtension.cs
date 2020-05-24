using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="ClassProperty"/>.
    /// </summary>
    public static class ClassPropertyExtension
    {
        /// <summary>
        /// Converts the list of <see cref="ClassProperty"/> into a a list of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="properties">The current instance of <see cref="ClassProperty"/>.</param>
        /// <returns>A list of <see cref="string"/> objects.</returns>
        public static IEnumerable<Field> AsFields(this IEnumerable<ClassProperty> properties)
        {
            foreach (var property in properties)
            {
                yield return property.AsField();
            }
        }

        /// <summary>
        /// Converts the list of <see cref="ClassProperty"/> into a a list of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="properties">The current instance of <see cref="ClassProperty"/>.</param>
        /// <returns>A list of <see cref="string"/> objects.</returns>
        public static IEnumerable<Field> AsFields(this IList<ClassProperty> properties) =>
            AsFields(properties.AsEnumerable());
    }
}
