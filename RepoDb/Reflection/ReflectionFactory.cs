using RepoDb.Attributes;
using System;
using System.Data.Common;
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
            switch (type)
            {
                case MethodInfoTypes.ConvertToStringMethod:
                    return TypeCache.Get(TypeTypes.Convert)
                        .GetMethod("ToString", TypeArrayCache.Get(TypeTypes.Object));
                case MethodInfoTypes.DataReaderGetItemMethod:
                    return TypeCache.Get(TypeTypes.DbDataReader)
                        .GetProperty("Item", TypeArrayCache.Get(TypeTypes.String)).GetMethod;
                case MethodInfoTypes.PropertySetValueMethod:
                    return TypeCache.Get(TypeTypes.PropertyInfo)
                        .GetMethod("SetValue", new[] { TypeCache.Get(TypeTypes.Object),
                            TypeCache.Get(TypeTypes.Object) });
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
                case TypeTypes.DbDataReader:
                    return typeof(DbDataReader);
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
        /// <param name="type">The type of Type array to be created.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] CreateTypes(params TypeTypes[] types)
        {
            var t = new Type[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                t[i] = TypeCache.Get(types[i]);
            }
            return t;
        }
    }
}
