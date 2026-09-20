#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.Vertica.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestVerticaTimeToDateTimePropertyHandler
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
        public void TestVerticaTimeToDateTimePropertyHandlerSet()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new VerticaTimeToDateTimePropertyHandler();
                var value = new DateTime(2026, 9, 20, 10, 30, 15);

                // Act
                var result = handler.Set(value, null);

                // Assert
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void TestVerticaTimeToDateTimePropertyHandlerGet()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new VerticaTimeToDateTimePropertyHandler();
                var value = new DateTime(2026, 9, 20, 10, 30, 15);

                // Act
                var result = handler.Get(value, null);

                // Assert
                Assert.AreEqual(default(DateTime).Add(new TimeSpan(10, 30, 15)), result);
                Assert.AreEqual(new TimeSpan(10, 30, 15), result.TimeOfDay);
            }
        }

        [TestMethod]
        public void TestVerticaTimeToDateTimePropertyHandlerGetWithMidnight()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new VerticaTimeToDateTimePropertyHandler();
                var value = new DateTime(2026, 9, 20);

                // Act
                var result = handler.Get(value, null);

                // Assert
                Assert.AreEqual(default(DateTime), result);
            }
        }

        [TestMethod]
        public void TestVerticaTimeToDateTimePropertyHandlerGetWithMaxTime()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new VerticaTimeToDateTimePropertyHandler();
                var value = new DateTime(2026, 9, 20, 23, 59, 59);

                // Act
                var result = handler.Get(value, null);

                // Assert
                Assert.AreEqual(new TimeSpan(23, 59, 59), result.TimeOfDay);
            }
        }

        [TestMethod]
        public void TestVerticaTimeToDateTimePropertyHandlerInsertAndQuery()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new VerticaTimeEntity { ColumnTime = new DateTime(2026, 9, 20, 10, 30, 15) };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<VerticaTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(new TimeSpan(10, 30, 15), result.ColumnTime.TimeOfDay);
            }
        }

        [TestMethod]
        public void TestVerticaTimeToDateTimePropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new VerticaTimeEntity { ColumnTime = new DateTime(2026, 9, 20, 1, 2, 3) },
                    new VerticaTimeEntity { ColumnTime = new DateTime(2026, 9, 20, 12, 0, 0) },
                    new VerticaTimeEntity { ColumnTime = new DateTime(2026, 9, 20, 23, 59, 59) }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<VerticaTimeEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(new TimeSpan(1, 2, 3), result[0].ColumnTime.TimeOfDay);
                Assert.AreEqual(new TimeSpan(12, 0, 0), result[1].ColumnTime.TimeOfDay);
                Assert.AreEqual(new TimeSpan(23, 59, 59), result[2].ColumnTime.TimeOfDay);
            }
        }
    }
}
