#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Resolvers;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class AuroraDbDbTypeToStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForTinyInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.TinyInt);

            // Assert
            Assert.AreEqual("TINYINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForSmallInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.SmallInt);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMediumInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MediumInt);

            // Assert
            Assert.AreEqual("MEDIUMINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Int);

            // Assert
            Assert.AreEqual("INT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForBigInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.BigInt);

            // Assert
            Assert.AreEqual("BIGINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForFloat()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForDouble()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForBit()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Char);

            // Assert
            Assert.AreEqual("CHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForEnum()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForSet()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForLongText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForBinary()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForDate()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForTime()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForYear()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForJson()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Json);

            // Assert
            Assert.AreEqual("JSON", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForPoint()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Point);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForLineString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.LineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForPolygon()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.Polygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMultiPoint()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MultiPoint);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMultiLineString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MultiLineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForMultiPolygon()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.MultiPolygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeToStringNameResolverForGeometryCollection()
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(AuroraDbType.GeometryCollection);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }
    }
}
