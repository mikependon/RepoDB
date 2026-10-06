#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;

namespace RepoDb.Schema.Vertica
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the Vertica schema objects for the <see cref="VerticaConnection"/> object.
    /// </summary>
    public static class VerticaSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the Vertica settings first (see <c>UseVertica</c>), then registers the <see cref="VerticaSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="VerticaConnection"/> type.
        /// A <see cref="VerticaSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseVerticaSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseVertica();
            SchemaComposerMapper.Add<VerticaConnection>(new VerticaSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
