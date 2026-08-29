using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class MariaDbDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBlobAsArray()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOBASARRAY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForLongBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMediumBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForTinyBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMultiLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTILINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMultiPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMultiPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForLongText()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMediumText()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NVARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForTinyText()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME2");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForTimeStamp()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDecimal2()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL2");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForMediumInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMINT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForYear()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("YEAR");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYINT");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new MariaDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
