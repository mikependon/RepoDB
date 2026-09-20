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
    public class TestDb2DecimalFloatToStringPropertyHandler
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
        public void TestDb2DecimalFloatToStringPropertyHandlerSetWithText()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatToStringPropertyHandler();
                var expected = new DB2DecimalFloat(123.456m);

                // Act
                var result = handler.Set("123.456", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(DB2DecimalFloat));
                Assert.AreEqual(expected.ToString(), result.ToString());
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatToStringPropertyHandlerSetWithNullOrEmpty()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Set(null, null);
                var resultOfEmpty = handler.Set("", null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfEmpty);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatToStringPropertyHandler();
                var expected = new DB2DecimalFloat(123.456m).ToString();

                // Act
                var resultOfDecimalFloat = handler.Get(new DB2DecimalFloat(123.456m), null);
                var resultOfDecimal = handler.Get(123.456m, null);
                var resultOfString = handler.Get("123.456", null);

                // Assert
                Assert.AreEqual(expected, resultOfDecimalFloat);
                Assert.AreEqual(expected, resultOfDecimal);
                Assert.AreEqual(expected, resultOfString);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2DecimalFloatToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2DecimalFloatToStringEntity { ColumnDecFloat = "123.456" };
                var expected = new DB2DecimalFloat(123.456m).ToString();

                // Act
                var id = Convert.ToInt32(connection.Insert(entity));
                var result = connection.Query<Db2DecimalFloatToStringEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(expected, result.ColumnDecFloat);
            }
        }

        [TestMethod]
        public void TestDb2DecimalFloatToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2DecimalFloatToStringEntity { ColumnDecFloat = null };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity));
                var result = connection.Query<Db2DecimalFloatToStringEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnDecFloat);
            }
        }
    }
}
