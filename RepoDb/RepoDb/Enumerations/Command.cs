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
        Create = 32,
        Drop = 64,
        Alter = 128,
        Execute = 256
    }
}
