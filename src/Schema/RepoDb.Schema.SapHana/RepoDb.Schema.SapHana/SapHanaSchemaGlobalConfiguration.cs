#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;

namespace RepoDb.Schema.SapHana
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the SapHana schema objects for the <see cref="HanaConnection"/> object.
    /// </summary>
    public static class SapHanaSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the SapHana settings first (see <c>UseSapHana</c>), then registers the <see cref="SapHanaSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="HanaConnection"/> type.
        /// A <see cref="SapHanaSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSapHanaSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseSapHana();
            SchemaComposerMapper.Add<HanaConnection>(new SapHanaSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
