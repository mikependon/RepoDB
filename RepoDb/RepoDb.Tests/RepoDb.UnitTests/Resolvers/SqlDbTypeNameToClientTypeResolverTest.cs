using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using RepoDb.Types;
using System;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class SqlDbTypeNameToClientTypeResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly SqlDbTypeNameToClientTypeResolver m_resolver = new SqlDbTypeNameToClientTypeResolver();

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var dbTypeName = "bigint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(long), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var dbTypeName = "binary";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var dbTypeName = "bit";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(bool), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var dbTypeName = "char";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var dbTypeName = "date";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var dbTypeName = "datetime";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var dbTypeName = "datetime2";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForDateTimeOffset()
        {
            // Setup
            var dbTypeName = "datetimeoffset";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var dbTypeName = "decimal";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Decimal), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForFileStream()
        {
            // Setup
            var dbTypeName = "filestream";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForAttribute()
        {
            // Setup
            var dbTypeName = "attribute";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForVarBinaryMax()
        {
            // Setup
            var dbTypeName = "varbinary(max)";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var dbTypeName = "float";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Double), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForImage()
        {
            // Setup
            var dbTypeName = "image";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var dbTypeName = "int";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(int), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForMoney()
        {
            // Setup
            var dbTypeName = "money";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var dbTypeName = "nchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForNText()
        {
            // Setup
            var dbTypeName = "ntext";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var dbTypeName = "numeric";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var dbTypeName = "nvarchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var dbTypeName = "real";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(float), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForRowVersion()
        {
            // Setup
            var dbTypeName = "rowversion";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForSmallDateTime()
        {
            // Setup
            var dbTypeName = "smalldatetime";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var dbTypeName = "smallint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(short), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForSmallMoney()
        {
            // Setup
            var dbTypeName = "smallmoney";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForSqlVariant()
        {
            // Setup
            var dbTypeName = "sql_variant";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(SqlVariant), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var dbTypeName = "text";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var dbTypeName = "time";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(TimeSpan), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var dbTypeName = "timestamp";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var dbTypeName = "tinyint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForUniqueIdentifier()
        {
            // Setup
            var dbTypeName = "uniqueidentifier";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Guid), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var dbTypeName = "varbinary";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var dbTypeName = "varchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlDbTypeNameToClientTypeResolverForXml()
        {
            // Setup
            var dbTypeName = "xml";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowOnExceptionTestSqlDbTypeNameToClientTypeResolverIfDbTypeNameIsNull()
        {
            // Act
            m_resolver.Resolve(null);
        }
    }
}
