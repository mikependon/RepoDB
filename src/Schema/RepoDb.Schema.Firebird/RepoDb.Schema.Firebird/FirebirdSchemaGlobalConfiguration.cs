#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Schema.Firebird
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class FirebirdSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Firebird settings first (see <c>UseFirebird</c>), then registers the reader and <see cref="FirebirdSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="FbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseFirebirdSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseFirebird();
            SchemaComposerMapper.Add<FbConnection>(new FirebirdSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<FbConnection>(new FirebirdSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<FbConnection>(FirebirdSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
