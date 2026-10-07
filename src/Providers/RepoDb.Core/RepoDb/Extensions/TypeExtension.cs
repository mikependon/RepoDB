#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using System;
using RepoDb.Attributes.Parameter;
using System.Collections.Generic;
using System.Data;
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
        /// Gets the corresponding <see cref="PropertyValueAttribute"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The instance of the <see cref="DbType"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> GetPropertyValueAttributes(this Type type)
        {
            return type != null ? PropertyValueAttributeMapper.Get(TypeCache.Get(type).GetUnderlyingType()) : null;
        }

        /// <summary>
        /// Gets the corresponding <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The instance of the <see cref="DbType"/> object.</returns>
        public static DbType? GetDbType(this Type type)
        {
            return type != null ? TypeMapCache.Get(TypeCache.Get(type).GetUnderlyingType()) : null;
        }

        /// <summary>
        /// Returns the instance of <see cref="ConstructorInfo"/> with the most argument.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The instance of <see cref="ConstructorInfo"/> with the most arguments.</returns>
        public static ConstructorInfo GetConstructorWithMostArguments([DynamicallyAccessedMembers(Trimming.Entity)] this Type type)
        {
            return type.GetConstructors().Where(item => item.GetParameters().Length > 0)
                .OrderByDescending(item => item.GetParameters().Length).FirstOrDefault();
        }

        /// <summary>
        /// Checks whether the current type is of type <see cref="object"/>.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a <see cref="object"/>.</returns>
        public static bool IsObjectType(this Type type)
        {
            return type == StaticType.Object;
        }

        /// <summary>
        /// Checks whether the current type is a class.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a class.</returns>
        public static bool IsClassType(this Type type)
        {
            return type.IsClass &&
!type.IsObjectType() &&
!StaticType.IEnumerable.IsAssignableFrom(type);
        }

        /// <summary>
        /// Checks whether the current type is an anonymous type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is an anonymous class.</returns>
        public static bool IsAnonymousType(this Type type)
        {
            return type.FullName.StartsWith("<>f__AnonymousType", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).</returns>
        public static bool IsDictionaryStringObject(this Type type)
        {
            return type == StaticType.IDictionaryStringObject ||
            type == StaticType.DictionaryStringObject || type == StaticType.ExpandoObject;
        }

        /// <summary>
        /// Checks whether the current type is wrapped within a <see cref="Nullable{T}"/> object.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is wrapped within a <see cref="Nullable{T}"/> object.</returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Checks whether the current type is a plain class type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a plain class type.</returns>
        internal static bool IsPlainType([DynamicallyAccessedMembers(Trimming.Entity)] this Type type)
        {
            var cachedType = TypeCache.Get(type);
            
            return (cachedType.IsClassType() || cachedType.IsAnonymousType()) &&
!IsQueryObjectType(type) &&
!cachedType.IsDictionaryStringObject() &&
!GetEnumerableClassProperties(type).Any();
        }

        /// <summary>
        /// Checks whether the current type is of type <see cref="QueryField"/> or <see cref="QueryGroup"/>.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is of type <see cref="QueryField"/> or <see cref="QueryGroup"/>.</returns>
        internal static bool IsQueryObjectType(this Type type)
        {
            return type == StaticType.QueryField || type == StaticType.QueryGroup;
        }

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> AsFields([DynamicallyAccessedMembers(Trimming.Entity)] this Type type)
        {
            return PropertyCache.Get(type).AsFields();
        }

        /// <summary>
        /// Gets the list of enumerable <see cref="ClassProperty"/> objects of the type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The list of the enumerable <see cref="ClassProperty"/> objects.</returns>
        internal static IEnumerable<ClassProperty> GetEnumerableClassProperties([DynamicallyAccessedMembers(Trimming.Entity)] this Type type)
        {
            return PropertyCache.Get(type).Where(classProperty =>
            {
                var propType = classProperty.PropertyInfo.PropertyType;
                return
                    propType != StaticType.String &&
                    propType != StaticType.CharArray &&
                    propType != StaticType.ByteArray &&
                    StaticType.IEnumerable.IsAssignableFrom(propType);
            });
        }

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> GetClassProperties([DynamicallyAccessedMembers(Trimming.Entity)] this Type type)
        {
            foreach (var property in TypeCache.GetProperties(type))
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
            return type != null ? (Nullable.GetUnderlyingType(type) ?? type) : null;
        }

        /// <summary>
        /// Returns the property of the type based on the mappings equality.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="mappedName">The name of the property mapping.</param>
        /// <returns>The instance of <see cref="ClassProperty"/>.</returns>
        internal static ClassProperty GetMappedProperty([DynamicallyAccessedMembers(Trimming.Entity)] this Type type,
            string mappedName)
        {
            return PropertyCache.Get(type)?.FirstOrDefault(p => string.Equals(p.GetMappedName(), mappedName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the list of the interface types being implemented by the current type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The list of the interface types.</returns>
        [Obsolete("Please use the Type.GetInterfaces() method instead.")]
        public static Type[] GetImplementedInterfaces([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        {
            return type?.GetInterfaces();
        }

        /// <summary>
        /// Creates a generic type of the current type based on the generic type available from the source type.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>The newly created generic type.</returns>
        [RequiresDynamicCode(Trimming.DynamicCodeMessage)]
        [RequiresUnreferencedCode("The generic type is constructed at runtime and its members might be trimmed.")]
        public static Type MakeGenericTypeFrom(this Type currentType,
            Type sourceType)
        {
            var genericTypes = sourceType?.GetGenericArguments();
            if (genericTypes?.Length == currentType?.GetGenericArguments().Length)
            {
                return currentType.MakeGenericType(genericTypes);
            }
            return null;
        }

        /// <summary>
        /// Checks whether the current type has implemented the target interface.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        /// <param name="interfaceType">The target interface type.</param>
        /// <returns>True if the current type has implemented the target interface.</returns>
        public static bool IsInterfacedTo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type currentType,
            Type interfaceType)
        {
            if (currentType == null || interfaceType == null)
            {
                return false;
            }
            if (!interfaceType.IsGenericTypeDefinition)
            {
                return interfaceType.IsAssignableFrom(currentType);
            }
            return IsGenericTypeOf(currentType, interfaceType) ||
                currentType.GetInterfaces().Any(item => IsGenericTypeOf(item, interfaceType));
        }

        /// <summary>
        /// Returns the type to be used when reflecting over the members of an object: the (annotated) generic type argument, or
        /// the runtime type of the object if it differs (i.e.: a derived type, or the generic type argument was widened to 'object').
        /// </summary>
        /// <typeparam name="T">The static type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The type to be used when reflecting over the members of the object.</returns>
        [return: DynamicallyAccessedMembers(Trimming.Entity)]
        internal static Type GetRuntimeType<[DynamicallyAccessedMembers(Trimming.Entity)] T>(T obj)
        {
            var type = obj?.GetType();
            return type == null || type == typeof(T) ? typeof(T) : GetRuntimeType((object)obj);
        }

        /// <summary>
        /// Returns the runtime type of an untyped object, to be used when reflecting over its members.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The runtime type of the object.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2073:Target return value does not satisfy 'DynamicallyAccessedMembersAttribute' requirements.",
            Justification = "RepoDB only reflects over the runtime type of an untyped object when it was passed via the object-based public APIs " +
                "(i.e.: the 'param', 'what', 'where' and 'entity' arguments of type object), which are annotated with RequiresUnreferencedCode, " +
                "or when it is an instance of a data entity type whose generic type argument is annotated (Trimming.Entity).")]
        [return: DynamicallyAccessedMembers(Trimming.Entity)]
        internal static Type GetRuntimeType(this object obj) =>
            obj?.GetType();

        /// <summary>
        /// Returns the type of a property/class handler instance: the (annotated) generic type argument, or the runtime type of
        /// the handler if it differs (i.e.: a derived type, or the generic type argument was widened).
        /// </summary>
        /// <typeparam name="THandler">The static type of the handler.</typeparam>
        /// <param name="handler">The handler instance.</param>
        /// <returns>The type of the handler.</returns>
        [return: DynamicallyAccessedMembers(Trimming.Handler)]
        internal static Type GetHandlerType<[DynamicallyAccessedMembers(Trimming.Handler)] THandler>(THandler handler)
        {
            var type = handler?.GetType();
            return type == null || type == typeof(THandler) ? typeof(THandler) : GetHandlerType((object)handler);
        }

        /// <summary>
        /// Returns the runtime type of an untyped property/class handler instance.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        /// <returns>The runtime type of the handler.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2073:Target return value does not satisfy 'DynamicallyAccessedMembersAttribute' requirements.",
            Justification = "The untyped handler registration APIs are annotated with RequiresUnreferencedCode; otherwise, the runtime type only " +
                "differs from the (annotated) generic type argument for derived handler types.")]
        [return: DynamicallyAccessedMembers(Trimming.Handler)]
        internal static Type GetHandlerType(object handler) =>
            handler?.GetType();

        /// <summary>
        /// Returns the default value of the type (i.e.: the zero-initialized instance of a value type, or null).
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The default value of the type.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method.",
            Justification = "The default value of a value type is its zero-initialized instance, no constructor is being invoked.")]
        internal static object GetDefaultValue(this Type type)
        {
            // The default value of a reference type or a Nullable<T> is null
            if (type?.IsValueType != true || Nullable.GetUnderlyingType(type) != null)
            {
                return null;
            }
#if NET
            return System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);
#else
            return Activator.CreateInstance(type);
#endif
        }

        /// <summary>
        /// Returns the <see cref="Nullable{T}"/> type of the current value type.
        /// </summary>
        /// <param name="type">The current value type.</param>
        /// <returns>The nullable type.</returns>
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Nullable<T> has no code of its own that needs to be generated for the instantiation; the NativeAOT runtime " +
                "can construct any Nullable<T> instantiation over an existing value type at runtime.")]
        internal static Type MakeNullableType(this Type type) =>
            typeof(Nullable<>).MakeGenericType(type);

        /// <summary>
        /// Checks whether the current type is a constructed type of the target generic type definition.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        /// <param name="genericTypeDefinition">The target generic type definition.</param>
        /// <returns>True if the current type is a constructed type of the target generic type definition.</returns>
        internal static bool IsGenericTypeOf(this Type currentType,
            Type genericTypeDefinition) =>
            currentType?.IsGenericType == true && currentType.GetGenericTypeDefinition() == genericTypeDefinition;

        /// <summary>
        /// Checks whether the current class handler type is valid to be used for the target model type.
        /// </summary>
        /// <param name="classHandlerType">The current class handler type type.</param>
        /// <param name="targetModelType">The target model type.</param>
        /// <returns>True if the current class handler type is valid to be used for the target model type.</returns>
        internal static bool IsClassHandlerValidForModel([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type classHandlerType,
            Type targetModelType)
        {
            var targetInterface = classHandlerType?
                .GetInterfaces()?
                .FirstOrDefault(item => string.Equals(item.Name, StaticType.IClassHandler.Name, StringComparison.Ordinal) && string.Equals(item.Namespace, StaticType.IClassHandler.Namespace, StringComparison.Ordinal));
            if (targetInterface != null)
            {
                return targetInterface.GetGenericArguments().FirstOrDefault() == targetModelType;
            }
            return false;
        }

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        public static int GenerateHashCode(Type type)
        {
            return type.GetHashCode();
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        public static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo)
        {
            return HashCode.Combine(entityType.GetHashCode(), propertyInfo.GenerateCustomizedHashCode(entityType));
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        public static PropertyInfo GetProperty<[DynamicallyAccessedMembers(Trimming.Entity)] T>(string propertyName)
            where T : class
        {
            return GetProperty(typeof(T), propertyName);
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="propertyName">The name of the target class property.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        public static PropertyInfo GetProperty([DynamicallyAccessedMembers(Trimming.Entity)] Type type,
            string propertyName)
        {
            return TypeCache.GetProperties(type)
                .FirstOrDefault(p =>
                    string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(p.GetMappedName(), propertyName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
