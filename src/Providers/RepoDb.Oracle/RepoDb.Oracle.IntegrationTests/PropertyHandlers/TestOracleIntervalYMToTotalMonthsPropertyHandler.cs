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
    public class TestOracleIntervalYMToTotalMonthsPropertyHandler
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
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerSetWithValue()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMToTotalMonthsPropertyHandler();

                // Act
                var result = handler.Set(14L, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OracleIntervalYM));
                Assert.AreEqual(14L, ((OracleIntervalYM)result).Value);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerSetWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMToTotalMonthsPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMToTotalMonthsPropertyHandler();

                // Act
                var resultOfInterval = handler.Get(new OracleIntervalYM(14), null);
                var resultOfLong = handler.Get(14L, null);
                var resultOfInt = handler.Get(14, null);

                // Assert
                Assert.AreEqual(14L, resultOfInterval);
                Assert.AreEqual(14L, resultOfLong);
                Assert.AreEqual(14L, resultOfInt);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerGetWithNullValues()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleIntervalYMToTotalMonthsPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);
                var resultOfNullInterval = handler.Get(OracleIntervalYM.Null, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
                Assert.IsNull(resultOfNullInterval);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleIntervalYMToTotalMonthsEntity { ColumnIntervalYm = 29L };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleIntervalYMToTotalMonthsEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(29L, result.ColumnIntervalYm);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerInsertAndQueryWithZero()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleIntervalYMToTotalMonthsEntity { ColumnIntervalYm = 0L };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleIntervalYMToTotalMonthsEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(0L, result.ColumnIntervalYm);
            }
        }

        [TestMethod]
        public void TestOracleIntervalYMToTotalMonthsPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleIntervalYMToTotalMonthsEntity { ColumnIntervalYm = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleIntervalYMToTotalMonthsEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnIntervalYm);
            }
        }
    }
}
