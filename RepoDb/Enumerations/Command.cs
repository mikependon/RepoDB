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
        /// Command used when querying a data (SELECT).
        /// </summary>
        Query = 2,
        /// <summary>
        /// Command used when inserting a data (INSERT).
        /// </summary>
        Insert = 4,
        /// <summary>
        /// Command used when updating a data (UPDATE).
        /// </summary>
        Update = 8,
        /// <summary>
        /// Command used when deleting a data (DELETE).
        /// </summary>
        Delete = 16,
        /// <summary>
        /// Command used when merging a data (MERGE).
        /// </summary>
        Merge = 32,
        /// <summary>
        /// Command used when doing a batch query (SELECT, ROW_NUMBER, OVER, BETWEEN).
        /// </summary>
        BatchQuery = 64,
        /// <summary>
        /// Command used when doing an inline update on the data (UPDATE).
        /// </summary>
        InlineUpdate = 128,
        /// <summary>
        /// Command used when bulk-inserting the data (UPDATE).
        /// </summary>
        BulkInsert = 256,
        /// <summary>
        /// Command used when counting a data (SELECT COUNT(*)).
        /// </summary>
        Count = 512,
        /// <summary>
        /// Command used when couting (big) a data (SELECT COUNT(*)).
        /// </summary>
        CountBig = 1024
    }
}
