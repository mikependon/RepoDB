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
            Name = name;
            IsPrimary = isPrimary;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Name { get; }

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
