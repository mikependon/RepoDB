namespace RepoDb
{
    /// <summary>
    /// A class used to define a mapping for the bulk insert operation.
    /// </summary>
    public class BulkInsertMapItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="BulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column.</param>
        /// <param name="destinationColumn">The name of the destination column.</param>
        public BulkInsertMapItem(string sourceColumn, string destinationColumn)
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
    }
}
