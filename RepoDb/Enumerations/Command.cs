using System;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define the commands for every operation.
    /// </summary>
    [Flags]
    public enum Command : short
    {
        /// <summary>
        /// No defined command and is applicable to every commands.
        /// </summary>
        None = 1,
        /// <summary>
        /// Command used when querying a data.
        /// </summary>
        Query = 2,
        /// <summary>
        /// Command used when inserting a data.
        /// </summary>
        Insert = 4,
        /// <summary>
        /// Command used when updating a data.
        /// </summary>
        Update = 8,
        /// <summary>
        /// Command used when deleting a data.
        /// </summary>
        Delete = 16,
        /// <summary>
        /// Command used when merging a data.
        /// </summary>
        Merge = 32,
        /// <summary>
        /// Command used when doing a batch query.
        /// </summary>
        BatchQuery = 64,
        /// <summary>
        /// Command used when doing an inline update on the data.
        /// </summary>
        InlineUpdate = 128,
        /// <summary>
        /// Command used when bulk-inserting the data.
        /// </summary>
        BulkInsert = 256,
        /// <summary>
        /// Command used when counting a data.
        /// </summary>
        Count = 512
    }
}
