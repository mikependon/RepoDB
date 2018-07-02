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
        /// A command that is being used when querying a data.
        /// </summary>
        Query = 2,
        /// <summary>
        /// A command that is being used when inserting a data.
        /// </summary>
        Insert = 4,
        /// <summary>
        /// A command that is being used when updating a data.
        /// </summary>
        Update = 8,
        /// <summary>
        /// A command that is being used when deleting a data.
        /// </summary>
        Delete = 16,
        /// <summary>
        /// A command that is being used when merging a data.
        /// </summary>
        Merge = 32,
        /// <summary>
        /// A command that is being used when batch-querying a data.
        /// </summary>
        BatchQuery = 64,
        /// <summary>
        /// A command that is being used when doing an inline update on the data.
        /// </summary>
        InlineUpdate = 128,
        /// <summary>
        /// A command that is being used when updating a data by targetting certain fields only.
        /// </summary>
        BulkInsert = 256,
        /// <summary>
        /// A command that is being used when counting the data.
        /// </summary>
        Count = 512,
        /// <summary>
        /// A command that is being used when deleting all data.
        /// </summary>
        DeleteAll = 1024,
        /// <summary>
        /// A command that is being used when inserting a data by targetting certain fields only.
        /// </summary>
        InlineInsert = 2048,
        /// <summary>
        /// A command that is being used when merging a data by targetting certain fields only.
        /// </summary>
        InlineMerge = 4096,
        /// <summary>
        /// A command that is being used when truncating a table.
        /// </summary>
        Truncate = 8192
    }
}
