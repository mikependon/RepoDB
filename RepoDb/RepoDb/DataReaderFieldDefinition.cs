using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to handle the field definition of the data reader.
    /// </summary>
    internal class DataReaderFieldDefinition
    {
        /// <summary>
        /// Gets or sets the handled column name value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the handled column ordinal value.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the handled column type value.
        /// </summary>
        public Type Type { get; set; }
    }
}
