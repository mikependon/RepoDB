#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Schema.Oracle
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class OracleSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Oracle settings first (see <c>UseOracle</c>), then registers the reader and <see cref="OracleSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="OracleConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseOracleSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseOracle();
            SchemaComposerMapper.Add<OracleConnection>(new OracleSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<OracleConnection>(new OracleSchemaReader(),
                force: true);
            SchemaDialectMapper.Add<OracleConnection>(OracleSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
