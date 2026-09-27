#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class AuroraDbDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBlobAsArray()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOBASARRAY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForLongBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMediumBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTinyBlob()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForLineString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMultiLineString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTILINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMultiPoint()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMultiPolygon()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForLongText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMediumText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NVARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTinyText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME2");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimeStamp()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDecimal2()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL2");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForMediumInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMINT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForYear()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("YEAR");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYINT");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
