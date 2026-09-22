#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using RepoDb.Db2.PropertyHandlers;
using System;
using System.Linq;

namespace RepoDb.Db2.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestDb2GuidToByteArrayPropertyHandler
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
        public void TestDb2GuidToByteArrayPropertyHandlerSet()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2GuidToByteArrayPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Set(guid, null);

                // Assert
                Assert.AreEqual(16, result.Length);
                CollectionAssert.AreEqual(guid.ToByteArray(), result);
            }
        }

        [TestMethod]
        public void TestDb2GuidToByteArrayPropertyHandlerGet()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2GuidToByteArrayPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Get(guid.ToByteArray(), null);

                // Assert
                Assert.AreEqual(guid, result);
            }
        }

        [TestMethod]
        public void TestDb2GuidToByteArrayPropertyHandlerGetWithNullOrEmpty()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2GuidToByteArrayPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfEmpty = handler.Get(Array.Empty<byte>(), null);

                // Assert
                Assert.AreEqual(Guid.Empty, resultOfNull);
                Assert.AreEqual(Guid.Empty, resultOfEmpty);
            }
        }

        [TestMethod]
        public void TestDb2GuidToByteArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2GuidEntity { ColumnGuid = Guid.NewGuid() };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<Db2GuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(entity.ColumnGuid, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestDb2GuidToByteArrayPropertyHandlerInsertAndQueryWithEmptyGuid()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2GuidEntity { ColumnGuid = Guid.Empty };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<Db2GuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestDb2GuidToByteArrayPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entities = Enumerable.Range(0, 3).Select(_ => new Db2GuidEntity { ColumnGuid = Guid.NewGuid() }).ToList();

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<Db2GuidEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEquivalent(entities.Select(e => e.ColumnGuid).ToList(), result.Select(e => e.ColumnGuid).ToList());
            }
        }
    }
}
