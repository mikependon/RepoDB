#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySqlConnector;

namespace RepoDb.Schema.MySqlConnector
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class MySqlConnectorSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the MySQL settings first (see <c>UseMySqlConnector</c>), then registers the reader and <see cref="MySqlConnectorSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MySqlConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySqlConnectorSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMySqlConnector();
            SchemaComposerMapper.Add<MySqlConnection>(new MySqlConnectorSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<MySqlConnection>(MySqlConnectorSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
