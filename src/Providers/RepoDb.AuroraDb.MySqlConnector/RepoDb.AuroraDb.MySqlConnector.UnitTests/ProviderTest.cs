#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.PropertyHandlers.AuroraDb;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests
{
    /// <summary>
    /// Tests for the bootstrapping, the resolvers, the truncate statement and the geometry property handler.
    /// </summary>
    [TestClass]
    public class ProviderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        #region Bootstrap

        [TestMethod]
        public void TestAuroraDbBootstrapMapsTheProviderObjects()
        {
            // Assert
            Assert.IsTrue(AuroraDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<AuroraDbDbSetting>(DbSettingMapper.Get<AuroraDbConnection>());
            Assert.IsInstanceOfType<AuroraDbDbHelper>(DbHelperMapper.Get<AuroraDbConnection>());
            Assert.IsInstanceOfType<AuroraDbStatementBuilder>(StatementBuilderMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestAuroraDbBootstrapIsIdempotent()
        {
            // Setup
            var dbSetting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            GlobalConfiguration
                .Setup()
                .UseAuroraDb()
                .UseAuroraDb();

            // Assert
            Assert.AreSame(dbSetting, DbSettingMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestAuroraDbBootstrapDoesNotMapTheUnderlyingDriverConnection()
        {
            // Assert (only the AuroraDbConnection is mapped, not the MySqlConnection of the AWS wrapper)
            Assert.IsNull(DbSettingMapper.Get<MySqlConnection>());
        }

        #endregion

        #region AuroraDbDbTypeNameToClientTypeResolver

        [TestMethod]
        [DataRow("BIGINT", typeof(long))]
        [DataRow("bigint", typeof(long))]
        [DataRow("BiGiNt", typeof(long))]
        [DataRow("VARCHAR", typeof(string))]
        [DataRow("json", typeof(string))]
        [DataRow("POINT", typeof(byte[]))]
        [DataRow("tinyint", typeof(sbyte))]
        [DataRow("bit", typeof(ulong))]
        public void TestAuroraDbDbTypeNameToClientTypeResolverIsCaseInsensitive(string dbTypeName,
            Type expected)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("none")]
        [DataRow("UNKNOWN_TYPE")]
        [DataRow("character varying")]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForUnknownTypes(string dbTypeName)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbDbTypeNameToClientTypeResolverIfTheNameIsNull()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion

        #region AuroraDbDbTypeToStringNameResolver

        [TestMethod]
        [DataRow(AuroraDbType.Enum, "TEXT")]
        [DataRow(AuroraDbType.Set, "TEXT")]
        [DataRow(AuroraDbType.GeometryCollection, "GEOMETRY")]
        [DataRow(AuroraDbType.MultiPolygon, "GEOMETRY")]
        [DataRow(AuroraDbType.Json, "JSON")]
        public void TestAuroraDbDbTypeToStringNameResolverForGroupedTypes(AuroraDbType dbType,
            string expected)
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected, result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForAnUndefinedValue()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve((AuroraDbType)short.MaxValue);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateTruncate("Table");

            // Assert
            Assert.AreEqual("TRUNCATE TABLE `Table` ;", query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateTruncateWithQuotedTableName()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateTruncate("`Table`");

            // Assert
            Assert.AreEqual("TRUNCATE TABLE `Table` ;", query, StringComparer.Ordinal);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateTruncateIfTheTableNameIsNullOrWhitespace(string tableName)
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => builder.CreateTruncate(tableName));
        }

        #endregion

        #region AuroraDbGeometryToMySqlGeometryPropertyHandler

        // A well-known binary (WKB) POINT(1 2), in little endian.
        private static readonly byte[] PointWkb = { 0x01, 0x01, 0x00, 0x00, 0x00, 0, 0, 0, 0, 0, 0, 0xF0, 0x3F, 0, 0, 0, 0, 0, 0, 0, 0x40 };

        [TestMethod]
        public void TestAuroraDbGeometryToMySqlGeometryPropertyHandlerGetForAGeometry()
        {
            // Setup
            var handler = new AuroraDbGeometryToMySqlGeometryPropertyHandler();
            var geometry = MySqlGeometry.FromWkb(4326, PointWkb);

            // Act
            var result = handler.Get(geometry, null);

            // Assert
            Assert.AreSame(geometry, result);
        }

        [TestMethod]
        public void TestAuroraDbGeometryToMySqlGeometryPropertyHandlerGetForTheInternalFormat()
        {
            // Setup (the MySQL internal format is a 4-byte SRID followed by the WKB)
            var handler = new AuroraDbGeometryToMySqlGeometryPropertyHandler();
            var bytes = MySqlGeometry.FromWkb(4326, PointWkb).Value;

            // Act
            var result = handler.Get(bytes, null);

            // Assert
            Assert.AreEqual(4326, result.SRID);
            CollectionAssert.AreEqual(PointWkb, result.WKB.ToArray());
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("POINT(1 2)")]
        [DataRow(12345)]
        public void TestAuroraDbGeometryToMySqlGeometryPropertyHandlerGetForUnsupportedValues(object input)
        {
            // Setup
            var handler = new AuroraDbGeometryToMySqlGeometryPropertyHandler();

            // Act/Assert
            Assert.IsNull(handler.Get(input, null));
            Assert.IsNull(handler.Get(DBNull.Value, null));
        }

        [TestMethod]
        public void TestAuroraDbGeometryToMySqlGeometryPropertyHandlerSet()
        {
            // Setup
            var handler = new AuroraDbGeometryToMySqlGeometryPropertyHandler();
            var geometry = MySqlGeometry.FromWkb(0, PointWkb);

            // Act/Assert
            Assert.AreSame(geometry, handler.Set(geometry, null));
            Assert.IsNull(handler.Set(null, null));
        }

        #endregion
    }
}
