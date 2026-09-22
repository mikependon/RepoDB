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
    public class TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler
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
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerSetWithValue()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(1970, 1, 1, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(FbZonedTime));
                Assert.AreEqual(value.UtcDateTime.TimeOfDay, ((FbZonedTime)result).Time);
                Assert.AreEqual("Etc/GMT-2", ((FbZonedTime)result).TimeZone, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerGetWithZonedTime()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler();
                var zonedTime = new FbZonedTime(new TimeSpan(8, 30, 15), "+02:00");

                // Act
                var result = handler.Get(zonedTime, null);

                // Assert
                Assert.AreEqual(new TimeSpan(10, 30, 15), ((DateTimeOffset)result).TimeOfDay);
                Assert.AreEqual(TimeSpan.FromHours(2), ((DateTimeOffset)result).Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerSetWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var value = new DateTimeOffset(1970, 1, 1, 10, 30, 15, TimeSpan.FromHours(2));
                var entity = new FirebirdNullableZonedTimeEntity { ColumnTimeTz = value };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdNullableZonedTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(value.TimeOfDay, result.ColumnTimeTz.Value.TimeOfDay);
                Assert.AreEqual(value.Offset, result.ColumnTimeTz.Value.Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedTimeToNullableDateTimeOffsetPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdNullableZonedTimeEntity { ColumnTimeTz = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdNullableZonedTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTimeTz);
            }
        }
    }
}
