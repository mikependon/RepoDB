using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Gets the description of the library.
        /// </summary>
        /// <returns>The description of the library.</returns>
        public static string GetAssemblyDescription()
        {
            /* RepoDb: A dynamic, lightweight, and very fast ORM .NET Library. */
            return typeof(TypeMapper)
                .Assembly
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute))?
                .OfType<AssemblyDescriptionAttribute>()?
                .FirstOrDefault()?
                .Description;
        }

        /// <summary>
        /// Gets a <see cref="Guid"/>-based random string.
        /// </summary>
        /// <returns>A <see cref="Guid"/>-based random string.</returns>
        public static string GetGuidBasedRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets a unicode string.
        /// </summary>
        /// <returns>A unicode string.</returns>
        public static string GetUnicodeString()
        {
            return "ÀÆÇÈËÌÏÐÑÒØÙÜÝÞß";
        }

        /// <summary>
        /// Gets the value of epoc date.
        /// </summary>
        /// <returns>An instance of date time value.</returns>
        public static DateTime GetEpocDate()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0);
        }
    }
}
