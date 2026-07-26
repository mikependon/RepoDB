namespace RepoDb.Enumerations.Oracle
{
    /// <summary>
    /// An enumeration that is being used to define the behavior of the identity property/column
    /// when an entity is being bulk-imported towards the underlying target table.
    /// </summary>
    /// <remarks>
    /// Unlike the PostgreSQL bulk package, this is the only enumeration this package exposes.
    /// There is no equivalent of <c>BulkImportPseudoTableType</c> - Oracle only ever stages rows via a
    /// session-scoped Global Temporary Table (GTT), since Oracle's DDL (CREATE/DROP TABLE) causes an
    /// implicit commit and therefore cannot safely be created/dropped per-call inside a caller's
    /// transaction the way PostgreSQL's physical-vs-TEMP pseudo table choice can. There is likewise no
    /// equivalent of <c>BulkImportMergeCommandType</c> - Oracle has exactly one native upsert construct
    /// (<c>MERGE INTO</c>), so there is no second strategy to choose between.
    /// </remarks>
    public enum BulkImportIdentityBehavior : short
    {
        /// <summary>
        /// No action required.
        /// </summary>
        Unspecified,

        /// <summary>
        /// A value that indicates whether the value of the identity property/column will be kept and used.
        /// </summary>
        KeepIdentity,

        /// <summary>
        /// A value that indicates whether the newly generated identity value from the target table will
        /// be set back to the entity.
        /// </summary>
        ReturnIdentity
    }
}
