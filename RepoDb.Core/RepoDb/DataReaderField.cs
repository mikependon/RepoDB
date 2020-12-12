using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the field definition of the data reader.
    /// </summary>
    internal class DataReaderField
    {
        /// <summary>
        /// Gets or sets the name value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the column ordinal value.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbField"/> value.
        /// </summary>
        public DbField DbField { get; set; }

        /// <summary>
        /// Gets or sets the type value.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The name of the field and the type.</returns>
        public override string ToString() =>
            string.Concat(Name, " (", Type?.FullName, ")");
    }
}
