#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.SapHana;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SapHana.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestSapHanaRealVectorToFloatArrayPropertyHandler
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
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerSetWithArray()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
                var value = new[] { 1.5f, 2.5f, 3.5f };

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.AreSame(value, result);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerSetWithNull()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithFloatArray()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
                var value = new[] { 1.5f, 2.5f, 3.5f };

                // Act
                var result = handler.Get(value, null);

                // Assert
                CollectionAssert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithBytes()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
                var expected = new[] { 1.5f, 2.5f, 3.5f };
                var bytes = expected.SelectMany(value => BitConverter.GetBytes(value)).ToArray();

                // Act
                var result = handler.Get(bytes, null);

                // Assert
                CollectionAssert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithBytesPrefixedByDimensions()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
                var expected = new[] { 1.5f, 2.5f, 3.5f };
                var bytes = BitConverter.GetBytes(3).Concat(expected.SelectMany(value => BitConverter.GetBytes(value))).ToArray();

                // Act
                var result = handler.Get(bytes, null);

                // Assert
                CollectionAssert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithNullValues()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithInvalidByteLength()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(new byte[5], null));
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                Database.EnsureRealVectorSupported();

                // Setup
                var entity = new SapHanaRealVectorEntity { ColumnRealVector = new[] { 1.5f, 2.5f, 3.5f } };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<SapHanaRealVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                CollectionAssert.AreEqual(entity.ColumnRealVector, result.ColumnRealVector);
            }
        }

        [TestMethod]
        public void TestSapHanaRealVectorToFloatArrayPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                Database.EnsureRealVectorSupported();

                // Setup
                var entity = new SapHanaRealVectorEntity { ColumnRealVector = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<SapHanaRealVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnRealVector);
            }
        }
    }
}
