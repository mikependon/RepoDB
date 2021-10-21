namespace RepoDb.Enumerations.PostgreSql
{
    /// <summary>
    /// An enumeration that is being used to define which command text to use when executing the 'BinaryBulkMerge' operation.
    /// </summary>
    public enum BulkImportMergeCommandType : short
    {
        /// <summary>
        /// An explicit 'INSERT' and 'UPDATE' commands will be used during the operation. It is the legacy 'UPSERT' operation.
        /// (This is the default value)
        /// </summary>
        InsertAndUpdate,
        /// <summary>
        /// An existing 'ON CONFLICT DO UPDATE' command will be used during the operation. By using this value, it  requires 
        /// that every entity only targets a single row from the underlying table, otherwise, an exception will be thrown. 
        /// To ensure that the operation is targetting the correct row, we highly recommend to always pass the value of the 
        /// primary column from your entities. If the 'qualifiers' argument is used, ensures that every qualifier field being 
        /// in-used is present from the table indexes, otherwise, a 'unique or exclusion constraint' exception will be thrown. 
        /// (The exceptions thrown are default from PostgreSQL)
        /// </summary>
        OnConflictDoUpdate
    }
}
