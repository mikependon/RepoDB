#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;

namespace RepoDb.Schema.PostgreSql
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the PostgreSQL schema objects for the <see cref="NpgsqlConnection"/> object.
    /// </summary>
    public static class PostgreSqlSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the PostgreSQL settings first (see <c>UsePostgreSql</c>), then registers the <see cref="PostgreSqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="NpgsqlConnection"/> type.
        /// A <see cref="PostgreSqlSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UsePostgreSqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UsePostgreSql();
            SchemaComposerMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
