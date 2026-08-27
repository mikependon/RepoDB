using System.Collections.ObjectModel;

namespace RepoDb.Firebird.BulkOperations
{
    /// <summary>
    /// A collection used to define the source-to-destination column mappings of a <see cref="FirebirdCommandBatcher"/>.
    /// </summary>
    public sealed class FirebirdCommandBatcherColumnMappingCollection : Collection<FirebirdCommandBatcherMapItem>
    {
        /// <summary>
        /// Adds a source-to-destination column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination table's column name.</param>
        /// <returns>The added mapping.</returns>
        public FirebirdCommandBatcherMapItem Add(
            string sourceColumn,
            string destinationColumn)
        {
            var mapping = new FirebirdCommandBatcherMapItem(sourceColumn, destinationColumn);
            Add(mapping);
            return mapping;
        }
    }
}
