#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Schema.AuroraDbPostgreSql
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class AuroraDbPostgreSqlSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the PostgreSQL settings first (see <c>UseAuroraDbPostgreSql</c>), then registers the reader and <see cref="AuroraDbPostgreSqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="AuroraDbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDbPostgreSqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseAuroraDb();
            SchemaComposerMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<AuroraDbConnection>(AuroraDbPostgreSqlSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
