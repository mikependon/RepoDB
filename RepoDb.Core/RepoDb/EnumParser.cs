using RepoDb.Extensions;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to parse the string values for enumeration.
    /// </summary>
    internal class EnumParser
    {
        /// <summary>
        /// Parses the string value to a desired enum. It uses the method <see cref="Enum.Parse(Type, string)"/> underneath.
        /// </summary>
        /// <param name="enumType">The type of enum.</param>
        /// <param name="value">The value to parse.</param>
        /// <param name="ignoreCase">The case sensitivity of the parse operation.</param>
        /// <returns>The enum value.</returns>
        public static object Parse(Type enumType,
            string value,
            bool ignoreCase)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Enum.Parse(enumType, value, ignoreCase);
            }
            if (enumType.IsNullable())
            {
                var nullable = typeof(Nullable<>).MakeGenericType(new[] { enumType });
                return Activator.CreateInstance(nullable);
            }
            else
            {
                return Activator.CreateInstance(enumType);
            }
        }
    }
}
