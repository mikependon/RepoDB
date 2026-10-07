#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Turso
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the SQLite schema objects for the <see cref="SqliteConnection"/> object.
    /// </summary>
    public static class TursoSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the Turso settings first (see <c>UseTurso</c>), then registers the <see cref="TursoSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqliteConnection"/> type.
        /// A <see cref="TursoSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseTursoSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseTurso();
            SchemaComposerMapper.Add<SqliteConnection>(new TursoSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
