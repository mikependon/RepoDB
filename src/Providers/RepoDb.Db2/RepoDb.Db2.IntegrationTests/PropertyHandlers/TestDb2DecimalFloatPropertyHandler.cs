#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using IBM.Data.DB2Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.Db2;
using System;
using System.Linq;

namespace RepoDb.Db2.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestDb2DecimalFloatPropertyHandler
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
        public void TestDb2DecimalFloatPropertyHandlerSetWithValue()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatPropertyHandler();
                var value = new DB2DecimalFloat(123.456m);

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(DB2DecimalFloat));
                Assert.AreEqual(value.ToString(), result.ToString());
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerSetWithNull()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatPropertyHandler();

                // Act
                var result = handler.Set(DB2DecimalFloat.Null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatPropertyHandler();
                var expected = new DB2DecimalFloat(123.456m);

                // Act
                var resultOfDecimalFloat = handler.Get(expected, null);
                var resultOfDecimal = handler.Get(123.456m, null);
                var resultOfString = handler.Get("123.456", null);

                // Assert
                Assert.IsFalse(resultOfDecimalFloat.IsNull);
                Assert.IsFalse(resultOfDecimal.IsNull);
                Assert.IsFalse(resultOfString.IsNull);
                Assert.AreEqual(expected.ToString(), resultOfDecimalFloat.ToString());
                Assert.AreEqual(expected.ToString(), resultOfDecimal.ToString());
                Assert.AreEqual(expected.ToString(), resultOfString.ToString());
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerGetWithNullValues()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);
                var resultOfEmptyString = handler.Get("", null);

                // Assert
                Assert.IsTrue(resultOfNull.IsNull);
                Assert.IsTrue(resultOfDbNull.IsNull);
                Assert.IsTrue(resultOfEmptyString.IsNull);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(new object(), null));
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2DecimalFloatEntity { ColumnDecFloat = new DB2DecimalFloat(123.456m) };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity));
                var result = connection.Query<Db2DecimalFloatEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsFalse(result.ColumnDecFloat.IsNull);
                Assert.AreEqual(entity.ColumnDecFloat.ToString(), result.ColumnDecFloat.ToString());
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2DecimalFloatEntity { ColumnDecFloat = DB2DecimalFloat.Null };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity));
                var result = connection.Query<Db2DecimalFloatEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsTrue(result.ColumnDecFloat.IsNull);
            }
        }
    }
}
