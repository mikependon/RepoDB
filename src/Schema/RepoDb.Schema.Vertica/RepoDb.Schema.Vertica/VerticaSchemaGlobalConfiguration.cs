#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Vertica.Data.VerticaClient;

namespace RepoDb.Schema.Vertica
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class VerticaSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Vertica settings first (see <c>UseVertica</c>), then registers the reader and <see cref="VerticaSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="VerticaConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseVerticaSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseVertica();
            SchemaComposerMapper.Add<VerticaConnection>(new VerticaSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<VerticaConnection>().Provider,
                VerticaSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
