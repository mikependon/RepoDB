#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using DuckDB.NET.Data;

namespace RepoDb.Schema.DuckDb
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class DuckDbSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the DuckDb settings first (see <c>UseDuckDb</c>), then registers the reader and <see cref="DuckDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="DuckDBConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDuckDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseDuckDb();
            SchemaComposerMapper.Add<DuckDBConnection>(new DuckDbSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<DuckDBConnection>(new DuckDbSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<DuckDBConnection>(DuckDbSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
