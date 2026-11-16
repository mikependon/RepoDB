#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Schema.SqlServer
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class SqlServerSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the SQL Server settings first (see <c>UseSqlServer</c>), then registers the reader and <see cref="SqlServerSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqlConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlServerSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSqlServer();
            SchemaComposerMapper.Add<SqlConnection>(new SqlServerSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<SqlConnection>(SqlServerSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
