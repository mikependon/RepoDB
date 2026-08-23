using System.Collections.ObjectModel;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// A collection used to define the source-to-destination column mappings of an <see cref="OracleBulkArrayBinder"/>,
    /// mirroring the shape of <see cref="OracleBulkCopyColumnMappingCollection"/>.
    /// </summary>
    public sealed class OracleBulkArrayBinderColumnMappingCollection : Collection<OracleBulkInsertMapItem>
    {
        /// <summary>
        /// Adds a source-to-destination column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination table's column name.</param>
        /// <returns>The added mapping.</returns>
        public OracleBulkInsertMapItem Add(
            string sourceColumn,
            string destinationColumn)
        {
            var mapping = new OracleBulkInsertMapItem(sourceColumn, destinationColumn);
            Add(mapping);
            return mapping;
        }
    }
}
