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
    public class TestOracleVectorToFloatArrayPropertyHandler
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
        public void TestOracleVectorToFloatArrayPropertyHandlerSetWithArray()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToFloatArrayPropertyHandler();
                var value = new float[] { 1.5f, 2.5f, 3.5f };

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OracleVector));
                Assert.IsFalse(((OracleVector)result).IsNull);
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerSetWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToFloatArrayPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToFloatArrayPropertyHandler();
                var expected = new float[] { 1.5f, 2.5f, 3.5f };

                // Act
                var resultOfSingleArray = handler.Get(new float[] { 1.5f, 2.5f, 3.5f }, null);
                var resultOfDoubleArray = handler.Get(new double[] { 1.5d, 2.5d, 3.5d }, null);
                var resultOfVector = handler.Get(new OracleVector(expected), null);

                // Assert
                CollectionAssert.AreEqual(expected, resultOfSingleArray);
                CollectionAssert.AreEqual(expected, resultOfDoubleArray);
                CollectionAssert.AreEqual(expected, resultOfVector);
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerGetWithNullValues()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToFloatArrayPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);
                var resultOfEmptyString = handler.Get("", null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
                Assert.IsNull(resultOfEmptyString);
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToFloatArrayPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Database.EnsureVectorSupported();

                // Setup
                var entity = new OracleVectorFloatEntity { ColumnVectorFloat = new float[] { 1.5f, 2.5f, 3.5f } };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleVectorFloatEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                CollectionAssert.AreEqual(entity.ColumnVectorFloat, result.ColumnVectorFloat);
            }
        }

        [TestMethod]
        public void TestOracleVectorToFloatArrayPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Database.EnsureVectorSupported();

                // Setup
                var entity = new OracleVectorFloatEntity { ColumnVectorFloat = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleVectorFloatEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnVectorFloat);
            }
        }
    }
}
