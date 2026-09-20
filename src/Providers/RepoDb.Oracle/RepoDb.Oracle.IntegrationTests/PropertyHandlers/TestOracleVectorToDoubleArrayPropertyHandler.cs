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
    public class TestOracleVectorToDoubleArrayPropertyHandler
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
        public void TestOracleVectorToDoubleArrayPropertyHandlerSetWithArray()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToDoubleArrayPropertyHandler();
                var value = new double[] { 1.5d, 2.5d, 3.5d };

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OracleVector));
                Assert.IsFalse(((OracleVector)result).IsNull);
            }
        }

        [TestMethod]
        public void TestOracleVectorToDoubleArrayPropertyHandlerSetWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToDoubleArrayPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestOracleVectorToDoubleArrayPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToDoubleArrayPropertyHandler();
                var expected = new double[] { 1.5d, 2.5d, 3.5d };

                // Act
                var resultOfDoubleArray = handler.Get(new double[] { 1.5d, 2.5d, 3.5d }, null);
                var resultOfSingleArray = handler.Get(new float[] { 1.5f, 2.5f, 3.5f }, null);
                var resultOfVector = handler.Get(new OracleVector(expected), null);

                // Assert
                CollectionAssert.AreEqual(expected, resultOfDoubleArray);
                CollectionAssert.AreEqual(expected, resultOfSingleArray);
                CollectionAssert.AreEqual(expected, resultOfVector);
            }
        }

        [TestMethod]
        public void TestOracleVectorToDoubleArrayPropertyHandlerGetWithNullValues()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToDoubleArrayPropertyHandler();

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
        public void TestOracleVectorToDoubleArrayPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleVectorToDoubleArrayPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestOracleVectorToDoubleArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleVectorDoubleEntity { ColumnVectorDouble = new double[] { 1.5d, 2.5d, 3.5d } };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleVectorDoubleEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                CollectionAssert.AreEqual(entity.ColumnVectorDouble, result.ColumnVectorDouble);
            }
        }

        [TestMethod]
        public void TestOracleVectorToDoubleArrayPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleVectorDoubleEntity { ColumnVectorDouble = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleVectorDoubleEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnVectorDouble);
            }
        }
    }
}
