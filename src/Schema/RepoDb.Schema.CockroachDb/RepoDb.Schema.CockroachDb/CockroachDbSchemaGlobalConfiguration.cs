#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb.Schema.CockroachDb
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class CockroachDbSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the CockroachDB settings first (see <c>UseCockroachDb</c>), then registers the reader and <see cref="CockroachDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="CockroachDbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseCockroachDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseCockroachDb();
            SchemaComposerMapper.Add<CockroachDbConnection>(new CockroachDbSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<CockroachDbConnection>(CockroachDbSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
