#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Dialects;

namespace RepoDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class PostgreSqlDbJsonDialectTest
    {
        #region ConfigureParameter

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new NpgsqlParameter();

            // Act
            new PostgreSqlDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(NpgsqlDbType.Jsonb, parameter.NpgsqlDbType);
        }

        #endregion
    }
}
