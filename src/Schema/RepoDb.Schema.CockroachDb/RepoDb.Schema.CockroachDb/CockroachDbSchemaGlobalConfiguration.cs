#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb.Schema.CockroachDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the CockroachDB schema objects for the <see cref="CockroachDbConnection"/> object.
    /// </summary>
    public static class CockroachDbSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the CockroachDB settings first (see <c>UseCockroachDb</c>), then registers the <see cref="CockroachDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="CockroachDbConnection"/> type.
        /// A <see cref="CockroachDbSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseCockroachDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseCockroachDb();
            SchemaComposerMapper.Add<CockroachDbConnection>(new CockroachDbSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
