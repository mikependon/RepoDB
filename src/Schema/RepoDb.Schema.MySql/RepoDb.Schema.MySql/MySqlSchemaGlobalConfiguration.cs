#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySql.Data.MySqlClient;

namespace RepoDb.Schema.MySql
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class MySqlSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the MySQL settings first (see <c>UseMySql</c>), then registers the reader and <see cref="MySqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MySqlConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMySql();
            SchemaComposerMapper.Add<MySqlConnection>(new MySqlSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<MySqlConnection>(new MySqlSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<MySqlConnection>(MySqlSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
