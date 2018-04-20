using System;
using System.Data.Common;
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
            switch (type)
            {
                case MethodInfoTypes.ConvertToStringMethod:
                    return TypeCache.Get(TypeTypes.ConvertType)
                        .GetMethod("ToString", TypeArrayCache.Get(TypeArrayTypes.ObjectTypes));
                case MethodInfoTypes.DataReaderGetItemMethod:
                    return TypeCache.Get(TypeTypes.DataReaderType)
                        .GetProperty("Item", TypeArrayCache.Get(TypeArrayTypes.StringTypes)).GetMethod;
                case MethodInfoTypes.PropertySetValueMethod:
                    return TypeCache.Get(TypeTypes.PropertyInfo)
                        .GetMethod("SetValue", new[] { TypeCache.Get(TypeTypes.ObjectType),
                            TypeCache.Get(TypeTypes.ObjectType) });
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a Type based on type.
        /// </summary>
        /// <param name="type">The Type to be created.</param>
        /// <returns>A type object.</returns>
        public static Type CreateType(TypeTypes type)
        {
            switch (type)
            {
                case TypeTypes.ConvertType:
                    return typeof(Convert);
                case TypeTypes.DataReaderType:
                    return typeof(DbDataReader);
                case TypeTypes.ExecutingAssemblyType:
                    return Assembly.GetExecutingAssembly().GetType();
                case TypeTypes.MethodInfo:
                    return typeof(MethodInfo);
                case TypeTypes.NullableGenericType:
                    return typeof(Nullable<>);
                case TypeTypes.ObjectType:
                    return typeof(object);
                case TypeTypes.PropertyInfo:
                    return typeof(PropertyInfo);
                case TypeTypes.StringType:
                    return typeof(string);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates an array of Types based on the type of passed.
        /// </summary>
        /// <param name="type">The type of Type array to be created.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] CreateTypes(TypeArrayTypes type)
        {
            switch (type)
            {
                case TypeArrayTypes.DataReaderTypes:
                    return new[] { TypeCache.Get(TypeTypes.DataReaderType) };
                case TypeArrayTypes.ObjectTypes:
                    return new[] { TypeCache.Get(TypeTypes.ObjectType) };
                case TypeArrayTypes.StringTypes:
                    return new[] { TypeCache.Get(TypeTypes.StringType) };
                default:
                    return null;
            }
        }
    }
}
