#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Microsoft
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class SqliteSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the SQLite settings first (see <c>UseSqlite</c>), then registers the reader and <see cref="SqliteSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqliteConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqliteSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSqlite();
            SchemaComposerMapper.Add<SqliteConnection>(new SqliteSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<SqliteConnection>(new SqliteSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<SqliteConnection>(SqliteSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
