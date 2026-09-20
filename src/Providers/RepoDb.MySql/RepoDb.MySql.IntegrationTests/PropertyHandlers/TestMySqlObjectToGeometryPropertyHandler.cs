#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.MySql;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestMySqlObjectToGeometryPropertyHandler
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
        public void TestMySqlObjectToGeometryPropertyHandlerSetWithValue()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MySqlObjectToGeometryPropertyHandler();
                var geometry = new MySqlGeometry(MySqlDbType.Geometry, CreatePointBytes(1.5, 2.5));

                // Act
                var result = handler.Set(geometry, null);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(MySqlGeometry));
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerSetWithNull()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MySqlObjectToGeometryPropertyHandler();

                // Act
                var result = handler.Set(new MySqlGeometry(true), null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerGetWithGeometry()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MySqlObjectToGeometryPropertyHandler();
                var geometry = new MySqlGeometry(MySqlDbType.Geometry, CreatePointBytes(1.5, 2.5));

                // Act
                var result = handler.Get(geometry, null);

                // Assert
                Assert.IsFalse(result.IsNull);
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerGetWithBytes()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MySqlObjectToGeometryPropertyHandler();

                // Act
                var result = handler.Get(CreatePointBytes(1.5, 2.5), null);

                // Assert
                Assert.IsFalse(result.IsNull);
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerGetWithNullValues()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new MySqlObjectToGeometryPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsTrue(resultOfNull.IsNull);
                Assert.IsTrue(resultOfDbNull.IsNull);
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerInsertAndQuery()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MySqlGeometryEntity
                {
                    ColumnGeometry = new MySqlGeometry(MySqlDbType.Geometry, CreatePointBytes(1.5, 2.5))
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<MySqlGeometryEntity>(e => e.Id == id).First();
                var text = connection.ExecuteScalar<string>("SELECT ST_AsText(`ColumnGeometry`) FROM `PropertyHandler` WHERE `Id` = @Id;", new { Id = id });

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsFalse(result.ColumnGeometry.IsNull);
                Assert.AreEqual("POINT(1.5 2.5)", text);
            }
        }

        [TestMethod]
        public void TestMySqlObjectToGeometryPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MySqlGeometryEntity
                {
                    ColumnGeometry = new MySqlGeometry(true)
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<MySqlGeometryEntity>(e => e.Id == id).First();
                var text = connection.ExecuteScalar<string>("SELECT ST_AsText(`ColumnGeometry`) FROM `PropertyHandler` WHERE `Id` = @Id;", new { Id = id });

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsTrue(result.ColumnGeometry.IsNull);
                Assert.IsNull(text);
            }
        }
    }
}
