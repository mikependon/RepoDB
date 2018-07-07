using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a foreign property for the recursive property of the <i>DataEntity</i> object.
    /// </summary>
    public class ForeignAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.ForeignAttribute</i> object.
        /// </summary>
        public ForeignAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name defined on this foreign attribute.
        /// </summary>
        public string Name { get; }
    }
}
