using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a primary property for the data entity object.
    /// </summary>
    public class PrimaryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrimaryAttribute"/> class.
        /// </summary>
        public PrimaryAttribute() { }
    }
}
