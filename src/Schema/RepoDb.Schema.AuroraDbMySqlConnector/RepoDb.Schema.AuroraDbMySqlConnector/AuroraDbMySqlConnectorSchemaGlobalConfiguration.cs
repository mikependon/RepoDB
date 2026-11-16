#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;

namespace RepoDb.Schema.AuroraDbMySqlConnector
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class AuroraDbMySqlConnectorSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the MySQL settings first (see <c>UseAuroraDbMySqlConnector</c>), then registers the reader and <see cref="AuroraDbMySqlConnectorSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="AuroraDbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDbMySqlConnectorSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseAuroraDb();
            SchemaComposerMapper.Add<AuroraDbConnection>(new AuroraDbMySqlConnectorSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbMySqlConnectorSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<AuroraDbConnection>().Provider,
                AuroraDbMySqlConnectorSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
