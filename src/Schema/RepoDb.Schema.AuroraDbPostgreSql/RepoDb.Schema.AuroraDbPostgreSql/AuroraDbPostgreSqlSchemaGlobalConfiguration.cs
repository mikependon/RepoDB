#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Schema.AuroraDbPostgreSql
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the PostgreSQL schema objects for the <see cref="AuroraDbConnection"/> object.
    /// </summary>
    public static class AuroraDbPostgreSqlSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the PostgreSQL settings first (see <c>UseAuroraDbPostgreSql</c>), then registers the <see cref="AuroraDbPostgreSqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="AuroraDbConnection"/> type.
        /// A <see cref="AuroraDbPostgreSqlSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDbPostgreSqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseAuroraDb();
            SchemaComposerMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
