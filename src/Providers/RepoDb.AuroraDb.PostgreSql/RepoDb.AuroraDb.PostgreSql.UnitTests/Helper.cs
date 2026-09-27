#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.AuroraDb.PostgreSql.UnitTests
{
    /// <summary>
    /// A helper class for the unit testing.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// A connection string that is never opened. The AWS wrapper behind the <see cref="Connector.AuroraDb.Npgsql.AuroraDbConnection"/>
        /// requires a connection string to be able to create a command, even if the connection itself is not opened.
        /// </summary>
        public const string ConnectionString = "Host=localhost;Port=5432;Database=RepoDb;Username=postgres;Password=RepoDB2026;";
    }
}
