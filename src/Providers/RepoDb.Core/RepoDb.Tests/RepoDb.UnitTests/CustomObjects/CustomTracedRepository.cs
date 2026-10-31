#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Exposes the entity repository with an instance connection for trace tests.
    /// </summary>
    public class CustomTracedRepository : BaseRepository<OperationTraceEntity, CustomTracedDbConnection>
    {
        #region Constructors

        /// <summary>
        /// Creates a repository using the supplied observer.
        /// </summary>
        /// <param name="trace">The observer receiving command notifications.</param>
        public CustomTracedRepository(ITrace trace)
            : base("ConnectionString", null, ConnectionPersistency.Instance, null, trace: trace)
        {
        }

        #endregion
    }
}
