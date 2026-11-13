#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Dialects;
using System.Text.Json;

namespace RepoDb.EnterpriseDb.UnitTests.Dialects
{
    [TestClass]
    public class EnterpriseDbDbJsonDialectTest
    {
        #region ConfigureParameter

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectConfigureParameterWithJsonText()
        {
            // Setup
            var parameter = new EDBParameter { Value = "{\"Id\":5}" };

            // Act
            new EnterpriseDbDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.IsInstanceOfType<JsonDocument>(parameter.Value);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectConfigureParameterWithNull()
        {
            // Setup
            var parameter = new EDBParameter { Value = System.DBNull.Value };

            // Act
            new EnterpriseDbDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(System.DBNull.Value, parameter.Value);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectConfigureParameterDbType()
        {
            // Setup
            var parameter = new EDBParameter { DbType = System.Data.DbType.String, Value = "{}" };

            // Act
            new EnterpriseDbDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(System.Data.DbType.Object, parameter.DbType);
        }

        #endregion
    }
}
