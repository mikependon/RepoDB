#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDb;

namespace RepoDb.Schema.MariaDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the MariaDB schema objects for the <see cref="MariaDbConnection"/> object.
    /// </summary>
    public static class MariaDbSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the MariaDB settings first (see <c>UseMariaDb</c>), then registers the <see cref="MariaDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MariaDbConnection"/> type.
        /// A <see cref="MariaDbSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMariaDb();
            SchemaComposerMapper.Add<MariaDbConnection>(new MariaDbSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
