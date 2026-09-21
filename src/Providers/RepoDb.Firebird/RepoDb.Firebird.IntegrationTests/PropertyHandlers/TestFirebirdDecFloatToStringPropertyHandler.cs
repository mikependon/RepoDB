#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.Firebird;
using System;
using System.Linq;

namespace RepoDb.Firebird.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestFirebirdDecFloatToStringPropertyHandler
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
        public void TestFirebirdDecFloatToStringPropertyHandlerSetWithText()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToStringPropertyHandler();

                // Act
                var result = handler.Set("123.45", null);

                // Assert
                Assert.AreEqual("123.45", handler.Get(result, null));
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToStringPropertyHandler();
                var decFloat = new FbDecFloat(new System.Numerics.BigInteger(12345), -2);

                // Act
                var resultOfDecFloat = handler.Get(decFloat, null);
                var resultOfDecimal = handler.Get(123.45m, null);
                var resultOfString = handler.Get("123.45", null);

                // Assert
                Assert.AreEqual("123.45", resultOfDecFloat);
                Assert.AreEqual("123.45", resultOfDecimal);
                Assert.AreEqual("123.45", resultOfString);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdDecFloatToStringEntity { ColumnDecFloat = "123.45" };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<FirebirdDecFloatToStringEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("123.45", result.ColumnDecFloat);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdDecFloatToStringEntity { ColumnDecFloat = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<FirebirdDecFloatToStringEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnDecFloat);
            }
        }
    }
}
