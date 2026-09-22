#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.ClickHouse;
using System;
using System.Linq;
using System.Text;

namespace RepoDb.ClickHouse.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestClickHouseFixedStringToStringPropertyHandler
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
        public void TestClickHouseFixedStringToStringPropertyHandlerSet()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();

                // Act
                var result = handler.Set("ABC", null);

                // Assert
                Assert.AreEqual("ABC", result);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerGetWithPaddedString()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();

                // Act
                var result = handler.Get("ABC\0\0\0\0\0", null);

                // Assert
                Assert.AreEqual("ABC", result, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerGetWithPaddedBytes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();
                var bytes = new byte[8];
                Encoding.UTF8.GetBytes("ABC").CopyTo(bytes, 0);

                // Act
                var result = handler.Get(bytes, null);

                // Assert
                Assert.AreEqual("ABC", result, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseFixedStringToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseFixedStringEntity { Id = 1, ColumnFixedString = "ABC" };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseFixedStringEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual("ABC", result.ColumnFixedString, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerInsertAndQueryWithFullLength()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseFixedStringEntity { Id = 1, ColumnFixedString = "ABCDEFGH" };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseFixedStringEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual("ABCDEFGH", result.ColumnFixedString, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseFixedStringToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseFixedStringEntity { Id = 1, ColumnFixedString = null };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseFixedStringEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.IsNull(result.ColumnFixedString);
            }
        }
    }
}
