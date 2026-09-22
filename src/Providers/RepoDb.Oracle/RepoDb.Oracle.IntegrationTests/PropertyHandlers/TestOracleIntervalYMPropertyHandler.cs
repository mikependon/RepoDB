#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.Oracle;
using System;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestOracleIntervalYMPropertyHandler
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerSetWithValue()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMPropertyHandler();
                var value = new OracleIntervalYM(14);

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OracleIntervalYM));
                Assert.AreEqual(14L, ((OracleIntervalYM)result).Value);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerSetWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMPropertyHandler();

                // Act
                var result = handler.Set(OracleIntervalYM.Null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMPropertyHandler();

                // Act
                var resultOfInterval = handler.Get(new OracleIntervalYM(14), null);
                var resultOfLong = handler.Get(14L, null);
                var resultOfInt = handler.Get(14, null);

                // Assert
                Assert.AreEqual(14L, resultOfInterval.Value);
                Assert.AreEqual(14L, resultOfLong.Value);
                Assert.AreEqual(14L, resultOfInt.Value);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerGetWithNullValues()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsTrue(resultOfNull.IsNull);
                Assert.IsTrue(resultOfDbNull.IsNull);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleIntervalYMEntity { ColumnIntervalYm = new OracleIntervalYM(2, 5) };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleIntervalYMEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsFalse(result.ColumnIntervalYm.IsNull);
                Assert.AreEqual(29L, result.ColumnIntervalYm.Value);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleIntervalYMEntity { ColumnIntervalYm = OracleIntervalYM.Null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleIntervalYMEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsTrue(result.ColumnIntervalYm.IsNull);
            }
        }
    }
}
