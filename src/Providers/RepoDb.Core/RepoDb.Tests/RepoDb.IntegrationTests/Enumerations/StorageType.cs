#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
