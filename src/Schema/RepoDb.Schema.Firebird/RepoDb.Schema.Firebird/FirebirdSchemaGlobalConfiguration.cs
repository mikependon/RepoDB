#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Schema.Firebird
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the Firebird schema objects for the <see cref="FbConnection"/> object.
    /// </summary>
    public static class FirebirdSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the Firebird settings first (see <c>UseFirebird</c>), then registers the <see cref="FirebirdSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="FbConnection"/> type.
        /// A <see cref="FirebirdSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseFirebirdSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseFirebird();
            SchemaComposerMapper.Add<FbConnection>(new FirebirdSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
