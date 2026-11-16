#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.Schema.MariaDbConnector
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class MariaDbConnectorSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the MariaDB settings first (see <c>UseMariaDbConnector</c>), then registers the reader and <see cref="MariaDbConnectorSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MariaDbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDbConnectorSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMariaDbConnector();
            SchemaComposerMapper.Add<MariaDbConnection>(new MariaDbConnectorSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<MariaDbConnection>(new MariaDbConnectorSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<MariaDbConnection>().Provider,
                MariaDbConnectorSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
