#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations;

namespace RepoDb.UnitTests.CustomObjects
{
    public sealed class CoverageRepository : BaseRepository<CoverageEntity, CoverageDbConnection>
    {
        public CoverageRepository()
            : base("UnitTest", null, ConnectionPersistency.Instance, null)
        {
        }
    }
}
