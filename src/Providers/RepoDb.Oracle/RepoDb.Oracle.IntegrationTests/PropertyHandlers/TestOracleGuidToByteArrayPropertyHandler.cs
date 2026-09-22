#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.Oracle.PropertyHandlers;
using System;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestOracleGuidToByteArrayPropertyHandler
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
        public void TestOracleGuidToByteArrayPropertyHandlerSet()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleGuidToByteArrayPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Set(guid, null);

                // Assert
                Assert.AreEqual(16, result.Length);
                CollectionAssert.AreEqual(guid.ToByteArray(), result);
            }
        }

        [TestMethod]
        public void TestOracleGuidToByteArrayPropertyHandlerGet()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleGuidToByteArrayPropertyHandler();
                var guid = Guid.NewGuid();

                // Act
                var result = handler.Get(guid.ToByteArray(), null);

                // Assert
                Assert.AreEqual(guid, result);
            }
        }

        [TestMethod]
        public void TestOracleGuidToByteArrayPropertyHandlerGetWithNullOrEmpty()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleGuidToByteArrayPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfEmpty = handler.Get(Array.Empty<byte>(), null);

                // Assert
                Assert.AreEqual(Guid.Empty, resultOfNull);
                Assert.AreEqual(Guid.Empty, resultOfEmpty);
            }
        }

        [TestMethod]
        public void TestOracleGuidToByteArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleGuidEntity { ColumnGuid = Guid.NewGuid() };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleGuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(entity.ColumnGuid, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestOracleGuidToByteArrayPropertyHandlerInsertAndQueryWithEmptyGuid()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleGuidEntity { ColumnGuid = Guid.Empty };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<OracleGuidEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.ColumnGuid);
            }
        }

        [TestMethod]
        public void TestOracleGuidToByteArrayPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Enumerable.Range(0, 3).Select(_ => new OracleGuidEntity { ColumnGuid = Guid.NewGuid() }).ToList();

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<OracleGuidEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEquivalent(entities.Select(e => e.ColumnGuid).ToList(), result.Select(e => e.ColumnGuid).ToList());
            }
        }
    }
}
