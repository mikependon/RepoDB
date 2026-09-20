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
    public class TestDb2ByteToInt16PropertyHandler
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
        public void TestDb2ByteToInt16PropertyHandlerSet()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2ByteToInt16PropertyHandler();

                // Act
                var result = handler.Set(200, null);

                // Assert
                Assert.AreEqual((short)200, result);
            }
        }

        [TestMethod]
        public void TestDb2ByteToInt16PropertyHandlerGet()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2ByteToInt16PropertyHandler();

                // Act
                var result = handler.Get(200, null);

                // Assert
                Assert.AreEqual((byte)200, result);
            }
        }

        [TestMethod]
        public void TestDb2ByteToInt16PropertyHandlerGetAndSetWithBoundaryValues()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2ByteToInt16PropertyHandler();

                // Act
                var min = handler.Get(handler.Set(byte.MinValue, null), null);
                var max = handler.Get(handler.Set(byte.MaxValue, null), null);

                // Assert
                Assert.AreEqual(byte.MinValue, min);
                Assert.AreEqual(byte.MaxValue, max);
            }
        }

        [TestMethod]
        public void TestDb2ByteToInt16PropertyHandlerInsertAndQuery()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2ByteEntity { ColumnTinyInt = 200 };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity));
                var result = connection.Query<Db2ByteEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual((byte)200, result.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestDb2ByteToInt16PropertyHandlerInsertAndQueryWithBoundaryValues()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var minEntity = new Db2ByteEntity { ColumnTinyInt = byte.MinValue };
                var maxEntity = new Db2ByteEntity { ColumnTinyInt = byte.MaxValue };

                // Act
                var minId = Convert.ToInt32(connection.Insert(minEntity));
                var maxId = Convert.ToInt32(connection.Insert(maxEntity));
                var minResult = connection.Query<Db2ByteEntity>(e => e.Id == minId).First();
                var maxResult = connection.Query<Db2ByteEntity>(e => e.Id == maxId).First();

                // Assert
                Assert.AreEqual(byte.MinValue, minResult.ColumnTinyInt);
                Assert.AreEqual(byte.MaxValue, maxResult.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestDb2ByteToInt16PropertyHandlerUpdate()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2ByteEntity { ColumnTinyInt = 10 };
                entity.Id = Convert.ToInt32(connection.Insert(entity));
                entity.ColumnTinyInt = 250;

                // Act
                var affectedRows = connection.Update(entity);
                var result = connection.Query<Db2ByteEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual((byte)250, result.ColumnTinyInt);
            }
        }
    }
}
