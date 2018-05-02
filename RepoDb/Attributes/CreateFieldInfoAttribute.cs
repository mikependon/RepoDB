using RepoDb.Reflection;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used by the RepoDb.FieldInfoTypes enum during the calls of RepoDb.ReflectionFactory.CreateField method.
    /// </summary>
    public class CreateFieldInfoAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance RepoDb.Attributes.CreateFieldInfoAttribute object.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="fieldName">The name of the field to retrieve.</param>
        public CreateFieldInfoAttribute(TypeTypes type, string fieldName)
        {
            Type = type;
            FieldName = fieldName;
        }

        /// <summary>
        /// The type of object.
        /// </summary>
        public TypeTypes Type { get; }

        /// <summary>
        /// The name of the field to retrieve.
        /// </summary>
        public string FieldName { get; }
    }
}