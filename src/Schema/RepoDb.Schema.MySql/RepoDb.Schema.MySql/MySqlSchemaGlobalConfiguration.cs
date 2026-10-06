#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;

namespace RepoDb.Schema.MySql
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the MySQL schema objects for the <see cref="MySqlConnection"/> object.
    /// </summary>
    public static class MySqlSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the MySQL settings first (see <c>UseMySql</c>), then registers the <see cref="MySqlSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MySqlConnection"/> type.
        /// A <see cref="MySqlSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySqlSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMySql();
            SchemaComposerMapper.Add<MySqlConnection>(new MySqlSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
