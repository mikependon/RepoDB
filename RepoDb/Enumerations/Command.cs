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
        /// A command that is being used when querying the data.
        /// </summary>
        Query = 2,
        /// <summary>
        /// A command that is being used when inserting the data.
        /// </summary>
        Insert = 4,
        /// <summary>
        /// A command that is being used when updating the data.
        /// </summary>
        Update = 8,
        /// <summary>
        /// A command that is being used when deleting the data.
        /// </summary>
        Delete = 16,
        /// <summary>
        /// A command that is being used when merging the data.
        /// </summary>
        Merge = 32,
        /// <summary>
        /// A command that is being used when querying the data (with batch).
        /// </summary>
        BatchQuery = 64,
        /// <summary>
        /// A command that is being used when doing an inline update on the data.
        /// </summary>
        InlineUpdate = 128,
        /// <summary>
        /// A command that is being used when updating the data, targetting certail fields only.
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
        /// A command that is being used when inserting the data, targetting certain fields only.
        /// </summary>
        InlineInsert = 2048,
        /// <summary>
        /// A command that is being used when merging the data, targetting certain fields only.
        /// </summary>
        InlineMerge = 4096,
        /// <summary>
        /// A command that is being used when truncating a table.
        /// </summary>
        Truncate = 8192
    }
}
