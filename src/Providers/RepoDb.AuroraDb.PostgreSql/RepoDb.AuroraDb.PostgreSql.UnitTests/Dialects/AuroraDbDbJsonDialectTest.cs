#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class AuroraDbDbJsonDialectTest
    {
        #region ConfigureParameter

        [TestMethod]
        public void TestAuroraDbDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new AuroraDbParameter();

            // Act
            new AuroraDbDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(AuroraDbType.Jsonb, parameter.AuroraDbType);
        }

        #endregion
    }
}
