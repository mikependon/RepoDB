namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define how the push operations (i.e.: Insert, InsertAll, Merge and MergeAll) behaves when returning the value from the column columns (i.e.: Primary and Identity).
    /// </summary>
    public enum KeyColumnReturnBehavior
    {
        /// <summary>
        /// Returns the value of the primary column.
        /// </summary>
        Primary = 1,
        /// <summary>
        /// Returns the value of the identity column.
        /// </summary>
        Identity = 2,
        /// <summary>
        /// Returns the coalesced value of the primary and identity columns.
        /// </summary>
        PrimaryOrElseIdentity = 3,
        /// <summary>
        /// Returns the coalesced value of the identity and primary columns.
        /// </summary>
        IdentityOrElsePrimary = 4,
    }
}
