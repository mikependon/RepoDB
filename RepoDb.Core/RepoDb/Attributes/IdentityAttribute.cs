using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define an identity property for the data entity object.
    /// </summary>
    public class IdentityAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IdentityAttribute"/> class.
        /// </summary>
        public IdentityAttribute() { }
    }
}
