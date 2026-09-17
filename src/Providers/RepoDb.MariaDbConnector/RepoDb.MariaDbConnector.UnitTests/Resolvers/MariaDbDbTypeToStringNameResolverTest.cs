#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.Resolvers;

namespace RepoDb.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class MariaDbDbTypeToStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDbConnector();
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyInt);

            // Assert
            Assert.AreEqual("TINYINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForSmallInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.SmallInt);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumInt);

            // Assert
            Assert.AreEqual("MEDIUMINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Int);

            // Assert
            Assert.AreEqual("INT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBigInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.BigInt);

            // Assert
            Assert.AreEqual("BIGINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForFloat()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDouble()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBit()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Char);

            // Assert
            Assert.AreEqual("CHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForEnum()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForSet()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLongText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDate()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForYear()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForJson()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Json);

            // Assert
            Assert.AreEqual("JSON", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Point);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Polygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiPoint);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiLineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiPolygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForGeometryCollection()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.GeometryCollection);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }
    }
}
