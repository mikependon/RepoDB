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
        /// Checks whether the current type is wrapped within <see cref="Nullable{T}"/> object.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is wrapped within <see cref="Nullable{T}"/> object.</returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> AsFields(this Type type) =>
            PropertyCache.Get(type).AsFields();

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> GetClassProperties(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                yield return new ClassProperty(type, property);
            }
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
        /// <returns>The instance of <see cref="ClassProperty"/>.</returns>
        internal static ClassProperty GetPropertyByMapping(this Type type,
            string mappedName)
        {
            return PropertyCache.Get(type)
                .FirstOrDefault(p => string.Equals(p.GetMappedName(), mappedName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks whether the current type has implemented the target interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="type">The current type.</param>
        /// <returns>True if the current type has implemented the target interface.</returns>
        public static bool IsInterfacedTo<T>(this Type type)
        {
            return IsInterfacedTo(type, typeof(T));
        }

        /// <summary>
        /// Checks whether the current type has implemented the target interface.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="interfaceType">The type of the interface.</param>
        /// <returns>True if the current type has implemented the target interface.</returns>
        public static bool IsInterfacedTo(this Type type,
            Type interfaceType)
        {
            if (interfaceType?.IsInterface != true)
            {
                throw new ArgumentException("The type of argument 'interfaceType' must be an interface.");
            }

            // Variables needed
            var interfaces = type
                .GetInterfaces()
                .Where(e => string.Equals(e.Namespace, interfaceType.Namespace))
                .AsList();
            var isInterfacedTo = false;

            // Iterates
            foreach (var item in interfaces)
            {
                // Check the type equality
                if (item == interfaceType)
                {
                    isInterfacedTo = true;
                    break;
                }

                // Check the generic arguments length
                if (AreGenericArgumentsLengthEquals(item, interfaceType) == false)
                {
                    continue;
                }

                // Check the members name
                if (AreMembersNamesEquals(item, interfaceType) == false)
                {
                    continue;
                }

                // Check the name equality
                if (item.FullName.Length > interfaceType.FullName.Length)
                {
                    isInterfacedTo = string.Equals(item.FullName.Substring(0, interfaceType.FullName.Length),
                        interfaceType.FullName, StringComparison.Ordinal);
                }
                else
                {
                    isInterfacedTo = string.Equals(interfaceType.FullName.Substring(0, item.FullName.Length),
                        item.FullName, StringComparison.Ordinal);
                }
                if (isInterfacedTo)
                {
                    break;
                }
            }

            // Return the value
            return isInterfacedTo;
        }

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateHashCode(Type type)
        {
            return type.GetUnderlyingType().FullName.GetHashCode();
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo)
        {
            return entityType.GetUnderlyingType().FullName.GetHashCode() + propertyInfo.GenerateCustomizedHashCode();
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(string propertyName)
            where T : class =>
            GetProperty(typeof(T), propertyName);

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty(Type type,
            string propertyName)
        {
            return type
                .GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks whether the generic arguments length are equal to both types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        /// <returns>True if the type of the generic arguments length are equal.</returns>
        internal static bool AreGenericArgumentsLengthEquals(Type type1,
            Type type2)
        {
            // Variables
            var genericArguments1 = type1?.GetGenericArguments();
            var genericArguments2 = type2?.GetGenericArguments();

            // Check nulls
            if (null == genericArguments1 && null == genericArguments2)
            {
                return true;
            }

            // Check the length equality
            if (genericArguments1?.Length != genericArguments2?.Length)
            {
                return false;
            }

            // Return True
            return true;
        }

        /// <summary>
        /// Checks whether the members names are equal to both types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        /// <returns>True if the type of the members names are equal.</returns>
        internal static bool AreMembersNamesEquals(Type type1,
            Type type2)
        {
            // Variables
            var members1 = type1?
                .GetMembers()
                .OrderBy(e => e.Name)
                .AsList();
            var members2 = type2?
                .GetMembers()
                .OrderBy(e => e.Name)
                .AsList();

            // Check nulls
            if (null == members1 && null == members2)
            {
                return true;
            }

            // Check the name equality
            if (members1?.Count() != members2?.Count())
            {
                return false;
            }
            else
            {
                for (var i = 0; i < members1.Count(); i++)
                {
                    if (string.Equals(members1[i].Name, members2[i].Name, StringComparison.Ordinal) == false)
                    {
                        return false;
                    }
                }
            }

            // Return True
            return true;
        }

        /// <summary>
        /// Checks whether the members length are equal to both types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        /// <returns>True if the type of the members length are equal.</returns>
        internal static bool AreMembersLengthEquals(Type type1,
            Type type2)
        {
            // Variables
            var members1 = type1?
                .GetMembers()
                .OrderBy(e => e.Name)
                .AsList();
            var members2 = type2?
                .GetMembers()
                .OrderBy(e => e.Name)
                .AsList();

            // Check nulls
            if (null == members1 && null == members2)
            {
                return true;
            }

            // Check the argument name equalities
            if (members1?.Count() != members2?.Count())
            {
                return false;
            }
            else
            {
                for (var i = 0; i < members1.Count(); i++)
                {
                    var member1 = members1[i];
                    var member2 = members2[i];

                    // Arguments length
                    if (MemberInfoExtension.IsMemberArgumentLengthEqual(member1, member2) == false)
                    {
                        return false;
                    }

                    // Check the name
                    if (string.Equals(member1.Name, member2.Name) == false)
                    {
                        return false;
                    }
                }
            }

            // Return True
            return true;
        }

        #endregion
    }
}
