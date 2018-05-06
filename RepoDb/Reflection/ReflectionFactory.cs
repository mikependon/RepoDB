using RepoDb.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A factory class used for certains reflection activity.
    /// </summary>
    public static class ReflectionFactory
    {
        /// <summary>
        /// Creates a System.Reflection.ConstructorInfo object of the defined type.
        /// </summary>
        /// <param name="type">The System.Type object where to create a constructor info.</param>
        /// <returns>A System.Reflection.ConstructorInfo object of the defined type.</returns>
        public static ConstructorInfo GetConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        /// <summary>
        /// Creates a System.Reflection.ConstructorInfo object of the defined type.
        /// </summary>
        /// <param name="type">The System.Type object  where to create a constructor info.</param>
        /// <param name="contructorTypes">The arguments of the constructor.</param>
        /// <returns>A System.Reflection.ConstructorInfo object of the defined type.</returns>
        public static ConstructorInfo GetConstructor(Type type, params Type[] contructorTypes)
        {
            return type.GetConstructor(contructorTypes);
        }

        /// <summary>
        /// Creates a System.Reflection.MethodInfo object based on type.
        /// </summary>
        /// <param name="type">A type of System.Reflection.MethodInfo object.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo CreateMethod(MethodInfoTypes type)
        {
            var createMethodInfoAttribute = typeof(MethodInfoTypes)
                .GetMembers()
                .First(member => member.Name.ToLower() == type.ToString().ToLower())
                .GetCustomAttribute<CreateMethodInfoAttribute>();
            var targetType = TypeCache.Get(createMethodInfoAttribute.Type);
            return createMethodInfoAttribute.ReflectedTypes == null ?
                targetType?.GetMethod(createMethodInfoAttribute.MethodName) :
                targetType?.GetMethod(createMethodInfoAttribute.MethodName, createMethodInfoAttribute.ReflectedTypes);
        }

        /// <summary>
        /// Creates a System.Reflection.FieldInfo object based on type.
        /// </summary>
        /// <param name="type">A type of System.Reflection.FieldInfo object.</param>
        /// <returns>A System.Reflection.FieldInfo object.</returns>
        public static FieldInfo CreateField(FieldInfoTypes type)
        {
            var createFieldInfoAttribute = typeof(FieldInfoTypes)
                .GetMembers()
                .First(member => member.Name.ToLower() == type.ToString().ToLower())
                .GetCustomAttribute<CreateFieldInfoAttribute>();
            return TypeCache.Get(createFieldInfoAttribute.Type)?.GetField(createFieldInfoAttribute.FieldName);
        }

        /// <summary>
        /// Creates a Type based on type.
        /// </summary>
        /// <param name="type">The type of Type to be created.</param>
        /// <returns>A type object.</returns>
        public static Type CreateType(TypeTypes type)
        {
            switch (type)
            {
                // TODO: Why is System.Data namespace is failing on a literal dynamic approach like below?
                case TypeTypes.DataRow:
                    return typeof(DataRow);
                case TypeTypes.DataTable:
                    return typeof(DataTable);
                case TypeTypes.DbDataReader:
                    return typeof(DbDataReader);
                case TypeTypes.DictionaryStringObject:
                    return typeof(IDictionary<string, object>);
                case TypeTypes.ExpandoObject:
                    return typeof(ExpandoObject);
                default:
                    var textAttribute = typeof(TypeTypes)
                        .GetMembers()
                        .First(member => member.Name.ToLower() == type.ToString().ToLower())
                        .GetCustomAttribute<TextAttribute>();
                    return Type.GetType(textAttribute.Text);
            }
        }

        /// <summary>
        /// Creates an array of Types based on the type of passed.
        /// </summary>
        /// <param name="types">The type of Type array to be created.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] CreateTypes(params TypeTypes[] types)
        {
            var length = Convert.ToInt32(types?.Length);
            var convertedTypes = new Type[length];
            for (var index = 0; index < length; index++)
            {
                convertedTypes[index] = TypeCache.Get(types[index]);
            }
            return convertedTypes;
        }
    }
}
