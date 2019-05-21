using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to handle the field definition of the data reader.
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
    }
}
