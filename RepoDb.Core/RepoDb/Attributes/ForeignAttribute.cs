using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a foreign relationship for the recursive property of the <i>DataEntity</i> object.
    /// </summary>
    public class ForeignAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.ForeignAttribute</i> object.
        /// </summary>
        /// <param name="childFieldName">The field name for the child <i>DataEntity</i> object.</param>
        public ForeignAttribute(string childFieldName) : this(null, childFieldName)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.ForeignAttribute</i> object.
        /// </summary>
        /// <param name="parentFieldName">The field name for the parent <i>DataEntity</i> object.</param>
        /// <param name="childFieldName">The field name for the child <i>DataEntity</i> object.</param>
        public ForeignAttribute(string parentFieldName, string childFieldName)
        {
            ParentFieldName = parentFieldName;
            ChildFieldName = childFieldName;
        }

        /// <summary>
        /// Gets the field name of the child data entity.
        /// </summary>
        public string ChildFieldName { get; }

        /// <summary>
        /// Gets the field name of the parent data entity.
        /// </summary>
        public string ParentFieldName { get; }
    }
}
