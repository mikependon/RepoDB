namespace RepoDb
{
    /// <summary>
    /// A class the holds the definition of the database table columns.
    /// </summary>
    internal class FieldDefinition
    {
        /// <summary>
        /// Creates a new instance of <see cref="FieldDefinition"/> object.
        /// </summary>
        public FieldDefinition() { }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value whether that identify whether the field is an identity.
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets the value that identify whether the field is nullable.
        /// </summary>
        public bool IsNullable { get; set; }
    }
}
