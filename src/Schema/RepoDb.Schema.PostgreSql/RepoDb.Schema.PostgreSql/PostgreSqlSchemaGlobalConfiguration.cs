#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Npgsql;

namespace RepoDb.Schema.PostgreSql
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class PostgreSqlSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the PostgreSQL settings first (see <c>UsePostgreSql</c>), then registers the reader and <see cref="PostgreSqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="NpgsqlConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UsePostgreSqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UsePostgreSql();
            SchemaComposerMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<NpgsqlConnection>(PostgreSqlSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
