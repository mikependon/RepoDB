#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Schema.SqlServer
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the SQL Server schema objects for the <see cref="SqlConnection"/> object.
    /// </summary>
    public static class SqlServerSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the SQL Server settings first (see <c>UseSqlServer</c>), then registers the <see cref="SqlServerSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqlConnection"/> type.
        /// A <see cref="SqlServerSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlServerSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSqlServer();
            SchemaComposerMapper.Add<SqlConnection>(new SqlServerSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
