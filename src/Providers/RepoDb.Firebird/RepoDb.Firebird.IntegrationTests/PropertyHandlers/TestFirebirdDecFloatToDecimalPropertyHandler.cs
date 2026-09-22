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
    public class TestFirebirdDecFloatToDecimalPropertyHandler
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
        public void TestFirebirdDecFloatToDecimalPropertyHandlerSet()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToDecimalPropertyHandler();

                // Act
                var result = handler.Set(123.45m, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(FbDecFloat));
                Assert.AreEqual(123.45m, decimal.Parse(result.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToDecimalPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToDecimalPropertyHandler();
                var decFloat = new FbDecFloat(new System.Numerics.BigInteger(12345), -2);

                // Act
                var resultOfDecFloat = handler.Get(decFloat, null);
                var resultOfDecimal = handler.Get(123.45m, null);
                var resultOfString = handler.Get("123.45", null);

                // Assert
                Assert.AreEqual(123.45m, resultOfDecFloat);
                Assert.AreEqual(123.45m, resultOfDecimal);
                Assert.AreEqual(123.45m, resultOfString);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToDecimalPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToDecimalPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.AreEqual(0m, resultOfNull);
                Assert.AreEqual(0m, resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToDecimalPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdDecFloatToDecimalPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(new object(), null));
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToDecimalPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdDecFloatToDecimalEntity { ColumnDecFloat = 123.45m };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdDecFloatToDecimalEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(123.45m, result.ColumnDecFloat);
            }
        }

        [TestMethod]
        public void TestFirebirdDecFloatToDecimalPropertyHandlerInsertAndQueryWithHighPrecision()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdDecFloatToDecimalEntity { ColumnDecFloat = 1234567890.123456789012345678m };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdDecFloatToDecimalEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(1234567890.123456789012345678m, result.ColumnDecFloat);
            }
        }
    }
}
