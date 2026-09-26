#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;
using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.MariaDbConnector;
using RepoDb.MariaDb.IntegrationTests.Models;
using RepoDb.MariaDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MariaDb.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandler
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

        /// <summary>
        /// Creates the MySQL internal binary format (4-byte SRID followed by the little-endian WKB) of a point.
        /// </summary>
        private static byte[] CreatePointBytes(double x,
            double y)
        {
            var bytes = new byte[25];
            bytes[4] = 1; // Little-endian
            bytes[5] = 1; // WKB point
            BitConverter.GetBytes(x).CopyTo(bytes, 9);
            BitConverter.GetBytes(y).CopyTo(bytes, 17);
            return bytes;
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerSetWithValue()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MariaDbConnectorGeometryToMySqlGeometryPropertyHandler();
                var geometry = MySqlGeometry.FromMySql(CreatePointBytes(1.5, 2.5));

                // Act
                var result = handler.Set(geometry, null);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(MySqlGeometry));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerSetWithNull()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MariaDbConnectorGeometryToMySqlGeometryPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerGetWithGeometry()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MariaDbConnectorGeometryToMySqlGeometryPropertyHandler();
                var geometry = MySqlGeometry.FromMySql(CreatePointBytes(1.5, 2.5));

                // Act
                var result = handler.Get(geometry, null);

                // Assert
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerGetWithBytes()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MariaDbConnectorGeometryToMySqlGeometryPropertyHandler();

                // Act
                var result = handler.Get(CreatePointBytes(1.5, 2.5), null);

                // Assert
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerGetWithNullValues()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MariaDbConnectorGeometryToMySqlGeometryPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerInsertAndQuery()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MariaDbConnectorGeometryEntity
                {
                    ColumnGeometry = MySqlGeometry.FromMySql(CreatePointBytes(1.5, 2.5))
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<MariaDbConnectorGeometryEntity>(e => e.Id == id).First();
                var text = connection.ExecuteScalar<string>("SELECT ST_AsText(`ColumnGeometry`) FROM `PropertyHandler` WHERE `Id` = @Id;", new { Id = id });

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNotNull(result.ColumnGeometry);
                Assert.AreEqual("POINT(1.5 2.5)", text, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectorGeometryToMySqlGeometryPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MariaDbConnectorGeometryEntity
                {
                    ColumnGeometry = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<MariaDbConnectorGeometryEntity>(e => e.Id == id).First();
                var isNull = connection.ExecuteScalar<long>("SELECT COUNT(1) FROM `PropertyHandler` WHERE `Id` = @Id AND `ColumnGeometry` IS NULL;", new { Id = id }) == 1;

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnGeometry);
                Assert.IsTrue(isNull);
            }
        }
    }
}
