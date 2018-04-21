using System;

namespace RepoDb.Enumerations
{
    [Flags]
    public enum Command : short
    {
        None = 1,
        Select = 2,
        Insert = 4,
        Update = 8,
        Delete = 16,
        Merge = 32,
        BulkInsert = 64,
        Count = 128,
        CountBig = 256
    }
}
