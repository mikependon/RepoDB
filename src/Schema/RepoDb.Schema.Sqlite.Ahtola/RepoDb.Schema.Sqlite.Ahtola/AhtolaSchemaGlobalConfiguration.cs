#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Ahtola.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Ahtola
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class AhtolaSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Ahtola settings first (see <c>UseAhtola</c>), then registers the reader and <see cref="AhtolaSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqliteConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAhtolaSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseAhtola();
            SchemaComposerMapper.Add<SqliteConnection>(new AhtolaSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<SqliteConnection>().Provider,
                AhtolaSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
