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
        /// Returns the coalesce value of the primary and identity columns.
        /// </summary>
        PrimaryOrIdentity = 3,
        /// <summary>
        /// Returns the coalesce value of the identity and primary columns.
        /// </summary>
        IdentityOrPrimary = 4,
        /// <summary>
        /// Returns the value of the identity column. If the identity column is not present, it will return the value of the primary column.
        /// </summary>
        IdentityOrElsePrimary = 5,
        /// <summary>
        /// Returns the value of the primary column. If the primary column is not present, it will return the value of the identity column.
        /// </summary>
        PrimaryOrElseIdentity = 6
    }
}
