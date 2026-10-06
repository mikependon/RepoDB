#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;

namespace RepoDb.Schema.DuckDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the DuckDb schema objects for the <see cref="DuckDBConnection"/> object.
    /// </summary>
    public static class DuckDbSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the DuckDb settings first (see <c>UseDuckDb</c>), then registers the <see cref="DuckDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="DuckDBConnection"/> type.
        /// A <see cref="DuckDbSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDuckDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseDuckDb();
            SchemaComposerMapper.Add<DuckDBConnection>(new DuckDbSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
