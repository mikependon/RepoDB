using System;
using System.ComponentModel.DataAnnotations;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a primary property for the data entity object.
    /// </summary>
    public class PrimaryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrimaryAttribute"/> class.
        /// </summary>
        public PrimaryAttribute() { }
    }
}
