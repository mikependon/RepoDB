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
    public class TestFirebirdZonedTimeToDateTimeOffsetPropertyHandler
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
        public void TestFirebirdZonedTimeToDateTimeOffsetPropertyHandlerSetWithValue()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(1970, 1, 1, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(FbZonedTime));
                Assert.AreEqual(value.UtcDateTime.TimeOfDay, ((FbZonedTime)result).Time);
                Assert.AreEqual("+02:00", ((FbZonedTime)result).TimeZone);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToDateTimeOffsetPropertyHandlerGetWithZonedTime()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToDateTimeOffsetPropertyHandler();
                var zonedTime = new FbZonedTime(new TimeSpan(8, 30, 15), "+02:00");

                // Act
                var result = handler.Get(zonedTime, null);

                // Assert
                Assert.AreEqual(new TimeSpan(10, 30, 15), ((DateTimeOffset)result).TimeOfDay);
                Assert.AreEqual(TimeSpan.FromHours(2), ((DateTimeOffset)result).Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToDateTimeOffsetPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToDateTimeOffsetPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.AreEqual(default(DateTimeOffset), resultOfNull);
                Assert.AreEqual(default(DateTimeOffset), resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToDateTimeOffsetPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToDateTimeOffsetPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToDateTimeOffsetPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var value = new DateTimeOffset(1970, 1, 1, 10, 30, 15, TimeSpan.FromHours(2));
                var entity = new FirebirdZonedTimeEntity { ColumnTimeTz = value };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<FirebirdZonedTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(value.TimeOfDay, result.ColumnTimeTz.TimeOfDay);
                Assert.AreEqual(value.Offset, result.ColumnTimeTz.Offset);
            }
        }
    }
}
