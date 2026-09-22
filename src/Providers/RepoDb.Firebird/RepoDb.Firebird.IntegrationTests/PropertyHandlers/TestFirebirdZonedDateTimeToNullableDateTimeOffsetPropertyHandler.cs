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
    public class TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler
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
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerSetWithValue()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(FirebirdSql.Data.Types.FbZonedDateTime));
                Assert.AreEqual(value, handler.Get(result, null));
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerGetWithDateTimeOffset()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler();
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));

                // Act
                var result = handler.Get(value, null);

                // Assert
                Assert.AreEqual(value, result);
                Assert.AreEqual(value.Offset, ((DateTimeOffset)result).Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerSetWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerGetWithNullValues()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerInsertAndQuery()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var value = new DateTimeOffset(2026, 9, 20, 10, 30, 15, TimeSpan.FromHours(2));
                var entity = new FirebirdNullableZonedDateTimeEntity { ColumnTimeStampTz = value };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdNullableZonedDateTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(value.UtcDateTime, result.ColumnTimeStampTz.Value.UtcDateTime);
                Assert.AreEqual(value.Offset, result.ColumnTimeStampTz.Value.Offset);
            }
        }

        [TestMethod]
        public void TestFirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new FirebirdNullableZonedDateTimeEntity { ColumnTimeStampTz = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<FirebirdNullableZonedDateTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTimeStampTz);
            }
        }
    }
}
