#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations.MySql;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for MySQL data provider, extended with the settings of the bulk operations
    /// (bulk-insert, bulk-merge, bulk-update and bulk-delete). It inherits all the values of <see cref="MySqlDbSetting"/>.
    /// Pass an instance to the <see cref="MySqlGlobalConfiguration.UseMySql(GlobalConfiguration, MySqlDbSetting)"/>
    /// method, which registers it as the setting of the MySQL connection.
    /// </summary>
    public sealed class MySqlBulkOperationsDbSetting : MySqlDbSetting
    {
        /// <summary>
        /// Gets or sets the value that defines how the columns of the source are aligned with the columns of the destination
        /// table when no explicit mappings were passed to a bulk operation. Explicit mappings always take precedence over this
        /// setting. The default value is <see cref="MySqlBulkColumnMappingsBehavior.Automatic"/>.
        /// </summary>
        public MySqlBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; } = MySqlBulkColumnMappingsBehavior.Automatic;
    }
}
