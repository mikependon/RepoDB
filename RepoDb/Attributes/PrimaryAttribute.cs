using System;

namespace RepoDb.Attributes
{
    public class PrimaryAttribute : Attribute
    {
        public PrimaryAttribute()
            : this(true) { }

        public PrimaryAttribute(bool isIdentity)
        {
            IsIdentity = isIdentity;
        }

        public bool IsIdentity { get; }
    }
}
