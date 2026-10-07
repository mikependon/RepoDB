#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Microsoft
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the SQLite schema objects for the <see cref="SqliteConnection"/> object.
    /// </summary>
    public static class SqliteSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the SQLite settings first (see <c>UseSqlite</c>), then registers the <see cref="SqliteSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqliteConnection"/> type.
        /// A <see cref="SqliteSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqliteSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSqlite();
            SchemaComposerMapper.Add<SqliteConnection>(new SqliteSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
