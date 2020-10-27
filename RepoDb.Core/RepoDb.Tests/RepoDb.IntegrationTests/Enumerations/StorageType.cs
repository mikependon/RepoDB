using System;

namespace RepoDb.IntegrationTests.Enumerations
{
    [Flags]
    public enum StorageType
    {
        File = 1,
        Folder = 2,
        Directory = 4,
        Drive = 8,
        InternalStorage = 16,
        MemoryStorage = 32
    }
}
