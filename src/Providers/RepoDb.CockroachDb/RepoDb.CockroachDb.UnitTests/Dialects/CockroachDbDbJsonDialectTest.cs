#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.CockroachDb.UnitTests.Dialects
{
    [TestClass]
    public class CockroachDbDbJsonDialectTest
    {
        #region ConfigureParameter

        [TestMethod]
        public void TestCockroachDbDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new CockroachDbParameter();

            // Act
            new CockroachDbDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(CockroachDbType.Jsonb, parameter.CockroachDbType);
        }

        #endregion
    }
}
