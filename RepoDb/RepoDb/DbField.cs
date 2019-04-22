using RepoDb.Extensions;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class the holds the column definition of the database table columns.
    /// </summary>
    public class DbField
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbField"/> object.
        /// </summary>
        public DbField(string name, bool isPrimary, bool isIdentity, bool isNullable)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException("Name");
            }

            // Set the properties
            Name = name.AsQuoted(true);
            UnquotedName = name;
            IsPrimary = isPrimary;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
        }

        /// <summary>
        /// Gets the quoted name of the database field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unquoted name of the database field.
        /// </summary>
        public string UnquotedName { get; }

        /// <summary>
        /// Gets the value whether the column is a primary column.
        /// </summary>
        public bool IsPrimary { get; }

        /// <summary>
        /// Gets the value whether the column is an identify column.
        /// </summary>
        public bool IsIdentity { get; }

        /// <summary>
        /// Gets the value whether the column is nullable.
        /// </summary>
        public bool IsNullable { get; }
    }
}
