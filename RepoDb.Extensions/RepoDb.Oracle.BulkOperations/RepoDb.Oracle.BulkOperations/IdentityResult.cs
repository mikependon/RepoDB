namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// Holds the correlation between an entity's original position in the input sequence (<see cref="Index"/>,
    /// backed by the staging table's <c>__RepoDb_OrderColumn</c>) and the identity value generated/matched for
    /// it on the target table (<see cref="Identity"/>). Used only by BulkMerge's post-MERGE identity lookup -
    /// BulkInsert reads its identities directly off a RETURNING ... INTO array and never needs this type.
    /// </summary>
    internal class IdentityResult
    {
        public int Index { get; set; }

        public decimal Identity { get; set; }
    }
}
