#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;

namespace RepoDb.Schema.Db2
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the Db2 schema objects for the <see cref="DB2Connection"/> object.
    /// </summary>
    public static class Db2SchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the Db2 settings first (see <c>UseDb2</c>), then registers the <see cref="Db2SchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="DB2Connection"/> type.
        /// A <see cref="Db2SchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDb2Schema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseDb2();
            SchemaComposerMapper.Add<DB2Connection>(new Db2SchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
