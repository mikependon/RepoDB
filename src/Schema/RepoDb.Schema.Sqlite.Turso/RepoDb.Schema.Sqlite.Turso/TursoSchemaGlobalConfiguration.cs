#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Turso.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Turso
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class TursoSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Turso settings first (see <c>UseTurso</c>), then registers the reader and <see cref="TursoSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqliteConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseTursoSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseTurso();
            SchemaComposerMapper.Add<SqliteConnection>(new TursoSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<SqliteConnection>(TursoSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
