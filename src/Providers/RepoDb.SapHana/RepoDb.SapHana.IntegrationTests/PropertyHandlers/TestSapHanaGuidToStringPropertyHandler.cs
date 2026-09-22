#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SapHana.PropertyHandlers;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SapHana.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestSapHanaGuidToStringPropertyHandler
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
        public void TestSapHanaGuidToStringPropertyHandlerSet()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaGuidToStringPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Set(guid, null);

                // Assert
                Assert.AreEqual(guid.ToString(), result, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerGet()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaGuidToStringPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Get(guid.ToString(), null);

                // Assert
                Assert.AreEqual(guid, result);
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerGetWithNullOrEmpty()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaGuidToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfEmpty = handler.Get("", null);

                // Assert
                Assert.AreEqual(Guid.Empty, resultOfNull);
                Assert.AreEqual(Guid.Empty, resultOfEmpty);
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerGetWithInvalidText()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SapHanaGuidToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<FormatException>(() => handler.Get("not-a-guid", null));
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new SapHanaGuidEntity { ColumnGuid = Guid.NewGuid() };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<SapHanaGuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(entity.ColumnGuid, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerInsertAndQueryWithEmptyGuid()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new SapHanaGuidEntity { ColumnGuid = Guid.Empty };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<SapHanaGuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestSapHanaGuidToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Enumerable.Range(0, 3).Select(_ => new SapHanaGuidEntity { ColumnGuid = Guid.NewGuid() }).ToList();

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<SapHanaGuidEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEquivalent(entities.Select(e => e.ColumnGuid).ToList(), result.Select(e => e.ColumnGuid).ToList());
            }
        }
    }
}
