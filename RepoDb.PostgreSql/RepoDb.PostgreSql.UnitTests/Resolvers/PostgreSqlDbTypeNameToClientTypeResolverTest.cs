using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class PostgreSqlDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            PostgreSqlBootstrap.Initialize();
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBlobAsArray()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOBASARRAY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForLongBlob()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMediumBlob()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTinyBlob()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYBLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBINARY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForLineString()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMultiLineString()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTILINESTRING");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMultiPoint()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMultiPolygon()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MULTIPOLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForLongText()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LONGTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMediumText()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NVARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTinyText()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYTEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME2");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimeStamp()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDecimal2()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL2");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMediumInt()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MEDIUMINT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForYear()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("YEAR");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYINT");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
