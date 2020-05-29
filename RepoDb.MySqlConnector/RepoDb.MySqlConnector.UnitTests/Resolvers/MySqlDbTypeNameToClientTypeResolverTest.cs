using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class MySqlDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            MySqlConnectorBootstrap.Initialize();
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBlobAsArray()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOBASARRAY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForLongBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMediumBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForTinyBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForLineString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMultiLineString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTILINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMultiPoint()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMultiPolygon()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForLongText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMediumText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NVARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForTinyText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME2");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForTimeStamp()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDecimal2()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL2");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForMediumInt()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMINT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForYear()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("YEAR");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYINT");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestMySqlDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
