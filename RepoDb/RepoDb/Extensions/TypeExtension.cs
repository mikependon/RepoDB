using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Converts all properties of the type into an array of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>A list of <see cref="string"/> objects.</returns>
        internal static IEnumerable<Field> AsFields(this Type type,
            IDbSetting dbSetting)
        {
            return PropertyCache.Get(type, dbSetting).AsFields();
        }

        /// <summary>
        /// Returns the underlying type of the current type. If there is no underlying type, this will return the current type.
        /// </summary>
        /// <param name="type">The current type to check.</param>
        /// <returns>The underlying type or the current type.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            return type != null ? Nullable.GetUnderlyingType(type) ?? type : null;
        }

        /// <summary>
        /// Returns the mapped property if the property is not present.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="mappedName">The name of the property mapping.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The instance of <see cref="ClassProperty"/>.</returns>
        internal static ClassProperty GetPropertyByMapping(this Type type,
            string mappedName,
            IDbSetting dbSetting)
        {
            return PropertyCache.Get(type, dbSetting)
                .FirstOrDefault(p => string.Equals(p.GetUnquotedMappedName(), mappedName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
