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
    public class TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandler
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
        public void TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandlerSetWithValue()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandlerGetWithDateTimeOffset()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Get(value, null);

                // Assert
                Assert.AreEqual(value, result);
                Assert.AreEqual(value.Offset, ((DateTimeOffset)result).Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToDateTimeOffsetPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.AreEqual(default(DateTimeOffset), resultOfNull);
                Assert.AreEqual(default(DateTimeOffset), resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToDateTimeOffsetPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToDateTimeOffsetPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));
                var entity = new FirebirdZonedDateTimeEntity { ColumnTimeStampTz = value };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<FirebirdZonedDateTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(value.UtcDateTime, result.ColumnTimeStampTz.UtcDateTime);
                Assert.AreEqual(value.Offset, result.ColumnTimeStampTz.Offset);
            }
        }
    }
}
