#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations.SqlServer;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for SQL Server data provider, extended with the settings of the bulk operations
    /// (bulk-insert, bulk-merge, bulk-update and bulk-delete). It inherits all the values of <see cref="SqlServerDbSetting"/>.
    /// Pass an instance to the <see cref="SqlServerGlobalConfiguration.UseSqlServer(GlobalConfiguration, SqlServerDbSetting)"/>
    /// method, which registers it as the setting of the SQL Server connection.
    /// </summary>
    public sealed class SqlServerBulkOperationsDbSetting : SqlServerDbSetting
    {
        /// <summary>
        /// Gets or sets the value that defines how the columns of the source are aligned with the columns of the destination
        /// table when no explicit mappings were passed to a bulk operation. Explicit mappings always take precedence over this
        /// setting. The default value is <see cref="SqlServerBulkColumnMappingsBehavior.Automatic"/>.
        /// </summary>
        public SqlServerBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; } = SqlServerBulkColumnMappingsBehavior.Automatic;
    }
}
