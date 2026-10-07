#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SqliteConnection"/> object.
    /// </summary>
    public static partial class TursoGlobalConfiguration
    {
        /// <summary>
        /// Initializes all necessary settings for Turso's SQLite-compatible connection.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseTurso(this GlobalConfiguration globalConfiguration)
        {
            TursoBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
