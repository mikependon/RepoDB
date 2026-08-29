namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk insert operation for SQL Server.
    /// </summary>
    public class SqlServerBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public SqlServerBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            base(sourceColumn, destinationColumn)
        { }

        #endregion
    }
}
