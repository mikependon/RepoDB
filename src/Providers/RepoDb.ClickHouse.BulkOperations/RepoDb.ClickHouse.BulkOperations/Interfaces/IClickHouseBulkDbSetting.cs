#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;

namespace RepoDb.ClickHouse.Interfaces
{
    /// <summary>
    /// Defines a contract for the ClickHouse database setting.
    /// </summary>
    public interface IClickHouseBulkDbSetting : IDbSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether waiting for mutations to complete is enabled for the ClickHouse database.
        /// </summary>
        public bool IsWaitForMutationsEnabled { get; set;}
    }
}
