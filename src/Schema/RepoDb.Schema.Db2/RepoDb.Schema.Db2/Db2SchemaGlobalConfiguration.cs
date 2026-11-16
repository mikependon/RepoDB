#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using IBM.Data.Db2;

namespace RepoDb.Schema.Db2
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class Db2SchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the Db2 settings first (see <c>UseDb2</c>), then registers the reader and <see cref="Db2SchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="DB2Connection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDb2Schema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseDb2();
            SchemaComposerMapper.Add<DB2Connection>(new Db2SchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<DB2Connection>(new Db2SchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<DB2Connection>().Provider,
                Db2SchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
