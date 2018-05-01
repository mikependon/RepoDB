using RepoDb.Reflection;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used by the RepoDb.MethodInfoTypes enum during the calls of RepoDb.ReflectionFactory.CreateMethod method.
    /// </summary>
    public class CreateMethodInfoAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance RepoDb.Attributes.CreateMethodInfoAttribute object.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="methodName">The name of the method to retrieve.</param>
        /// <param name="types">The array of types when reflecting the methods.</param>
        public CreateMethodInfoAttribute(TypeTypes type, string methodName, params Type[] types)
        {
            Type = type;
            MethodName = methodName;
            ReflectedTypes = types;
        }

        /// <summary>
        /// The type of object.
        /// </summary>
        public TypeTypes Type { get; }

        /// <summary>
        /// The name of the method to retrieve.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// The array of types when reflecting the methods.
        /// </summary>
        public Type[] ReflectedTypes { get; }
    }
}