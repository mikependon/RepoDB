namespace RepoDb.Enumerations.ClickHouse
{
    /// <summary>
    /// An enumeration that is being used to define the behavior of the identity property/column when an entity is being bulk-imported towards the underlying target table.
    /// </summary>
    public enum ClickHouseBulkImportIdentityBehavior : short
    {
        /// <summary>
        /// A value that indicates whether the value of the identity property/column will be kept and used.
        /// This is the only supported behavior for ClickHouse.
        /// </summary>
        KeepIdentity,

        /// <summary>
        /// A value that indicates whether the newly generated identity value from the target table will
        /// be set back to the entity. Not supported for ClickHouse: ClickHouse has no session-wide scope
        /// identity, sequence, or auto-increment mechanism of any kind (see also
        /// <c>RepoDb.DbHelpers.ClickHouseDbHelper.GetScopeIdentity</c> in <c>RepoDb.ClickHouse</c>), so
        /// requesting this value throws a <see cref="NotSupportedException"/>.
        /// </summary>
        ReturnIdentity
    }
}
