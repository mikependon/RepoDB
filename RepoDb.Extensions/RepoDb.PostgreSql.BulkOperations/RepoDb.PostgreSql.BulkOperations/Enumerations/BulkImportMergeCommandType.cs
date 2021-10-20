namespace RepoDb.Enumerations.PostgreSql
{
    /// <summary>
    /// An enumeration that is being used to define which command text to use when executing the 'BinaryBulkMerge' operation.
    /// </summary>
    public enum BulkImportMergeCommandType : short
    {
        /// <summary>
        /// An explicit 'INSERT' and 'UPDATE' command will be used during the operation. It is the legacy 'UPSERT' operation.
        /// (This is the default value)
        /// </summary>
        InsertAndUpdate,
        /// <summary>
        /// An existing 'ON CONFLICT DO UPDATE' command will be used during the operation. Using this value requires your qualifiers
        /// to be part of the table index. If the qualifiers provided are not indexed, a 'unique or exclusion constraint' exception 
        /// will be thrown by the PostgeSQL.
        /// </summary>
        OnConflictDoUpdate
    }
}
