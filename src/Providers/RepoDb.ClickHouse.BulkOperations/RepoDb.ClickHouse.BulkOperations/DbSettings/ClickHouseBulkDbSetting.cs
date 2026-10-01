#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.Interfaces;
using RepoDb.Enumerations.ClickHouse;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="ClickHouseConnection"/> data provider with properties for bulk operations.
    /// </summary>
    public sealed class ClickHouseBulkDbSetting : ClickHouseDbSetting, IClickHouseBulkDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseBulkDbSetting"/> class.
        /// </summary>
        public ClickHouseBulkDbSetting()
            : base()
        {}

        /// <summary>
        /// Gets or sets a value indicating whether waiting for mutations to complete is enabled for the ClickHouse database.
        /// </summary>
        public bool IsWaitForMutationsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the value that defines how the columns of the source are aligned with the columns of the destination
        /// table when no explicit mappings were passed to a bulk operation. Explicit mappings always take precedence over this
        /// setting. The default value is <see cref="ClickHouseBulkColumnMappingsBehavior.Automatic"/>.
        /// </summary>
        public ClickHouseBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; } = ClickHouseBulkColumnMappingsBehavior.Automatic;
    }
}
