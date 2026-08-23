using System.Collections.ObjectModel;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// A collection used to define the source-to-destination column mappings of an <see cref="OracleBulkArrayBinder"/>,
    /// mirroring the shape of <see cref="OracleBulkCopyColumnMappingCollection"/>.
    /// </summary>
    internal sealed class OracleBulkArrayBinderColumnMappingCollection : Collection<OracleBulkInsertMapItem>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceColumn"></param>
        /// <param name="destinationColumn"></param>
        /// <returns></returns>
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
