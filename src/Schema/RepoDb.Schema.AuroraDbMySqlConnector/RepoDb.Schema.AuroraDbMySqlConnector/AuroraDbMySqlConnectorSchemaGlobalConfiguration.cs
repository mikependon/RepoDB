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
    /// A class that is being used to initialize the necessary settings of the MySQL schema objects for the <see cref="AuroraDbConnection"/> object.
    /// </summary>
    public static class AuroraDbMySqlConnectorSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the MySQL settings first (see <c>UseAuroraDbMySqlConnector</c>), then registers the <see cref="AuroraDbMySqlConnectorSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="AuroraDbConnection"/> type.
        /// A <see cref="AuroraDbMySqlConnectorSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDbMySqlConnectorSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseAuroraDb();
            SchemaComposerMapper.Add<AuroraDbConnection>(new AuroraDbMySqlConnectorSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
