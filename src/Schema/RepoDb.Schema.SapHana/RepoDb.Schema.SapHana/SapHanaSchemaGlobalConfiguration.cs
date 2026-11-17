#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Sap.Data.Hana;

namespace RepoDb.Schema.SapHana
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class SapHanaSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the SapHana settings first (see <c>UseSapHana</c>), then registers the reader and <see cref="SapHanaSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="HanaConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSapHanaSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSapHana();
            SchemaComposerMapper.Add<HanaConnection>(new SapHanaSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<HanaConnection>(new SapHanaSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<HanaConnection>(SapHanaSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
