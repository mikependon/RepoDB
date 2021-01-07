namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk insert operation.
    /// </summary>
    public class BulkInsertMapItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="BulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public BulkInsertMapItem(string sourceColumn,
            string destinationColumn)
        {
            SourceColumn = sourceColumn;
            DestinationColumn = destinationColumn;
        }

        /// <summary>
        /// Gets the name of the source column.
        /// </summary>
        public string SourceColumn { get; }

        /// <summary>
        /// Gets the name of the destination column.
        /// </summary>
        public string DestinationColumn { get; }

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"Source: {SourceColumn} = Destination: {DestinationColumn}";
    }
}
