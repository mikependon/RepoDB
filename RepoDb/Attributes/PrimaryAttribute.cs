using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a primary property for the DTO (<i>Data Transfer Object</i>) class.
    /// </summary>
    public class PrimaryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.PrimaryAttribute</i> object.
        /// </summary>
        public PrimaryAttribute()
            : this(true) { }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.PrimaryAttribute</i> object.
        /// </summary>
        /// <param name="isIdentity">Used to define whether the primary property is an identity.</param>
        public PrimaryAttribute(bool isIdentity)
        {
            IsIdentity = isIdentity;
        }

        /// <summary>
        /// Gets a value that defines whether the primary property is an identity.
        /// </summary>
        public bool IsIdentity { get; }
    }
}
