using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Reflection.MemberInfo</i> object.
    /// </summary>
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Gets a custom attribute defined on the member.
        /// </summary>
        /// <typeparam name="T">The custom attribute that is defined into the property.</typeparam>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <returns>The custom attribute.</returns>
        public static T GetCustomAttribute<T>(this MemberInfo property) where T : Attribute
        {
            return (T)GetCustomAttribute(property, typeof(T));
        }

        /// <summary>
        /// Gets a custom attribute defined on the member.
        /// </summary>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <param name="type">The custom attribute that is defined into the member.</param>
        /// <returns>The custom attribute.</returns>
        public static Attribute GetCustomAttribute(this MemberInfo property, Type type)
        {
            var attributes = property.GetCustomAttributes(type, false);
            return attributes?.Any() == true ? (Attribute)attributes[0] : null;
        }
    }
}
