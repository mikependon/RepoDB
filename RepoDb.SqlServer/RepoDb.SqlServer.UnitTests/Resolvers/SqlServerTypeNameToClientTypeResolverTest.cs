using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using RepoDb.Types;
using System;

namespace RepoDb.SqlServer.UnitTests.Resolvers
{
    [TestClass]
    public class SqlServerTypeNameToClientTypeResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly SqlServerDbTypeNameToClientTypeResolver m_resolver = new();

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var dbTypeName = "bigint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(long), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var dbTypeName = "binary";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var dbTypeName = "bit";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(bool), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var dbTypeName = "char";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var dbTypeName = "date";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var dbTypeName = "datetime";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForDateTime2()
        {
            // Setup
            var dbTypeName = "datetime2";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForDateTimeOffset()
        {
            // Setup
            var dbTypeName = "datetimeoffset";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var dbTypeName = "decimal";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Decimal), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForFileStream()
        {
            // Setup
            var dbTypeName = "filestream";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForAttribute()
        {
            // Setup
            var dbTypeName = "attribute";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForVarBinaryMax()
        {
            // Setup
            var dbTypeName = "varbinary(max)";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var dbTypeName = "float";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Double), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForImage()
        {
            // Setup
            var dbTypeName = "image";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var dbTypeName = "int";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(int), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForMoney()
        {
            // Setup
            var dbTypeName = "money";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForNChar()
        {
            // Setup
            var dbTypeName = "nchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForNText()
        {
            // Setup
            var dbTypeName = "ntext";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var dbTypeName = "numeric";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForNVarChar()
        {
            // Setup
            var dbTypeName = "nvarchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var dbTypeName = "real";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(float), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForRowVersion()
        {
            // Setup
            var dbTypeName = "rowversion";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForSmallDateTime()
        {
            // Setup
            var dbTypeName = "smalldatetime";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(DateTime), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var dbTypeName = "smallint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(short), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForSmallMoney()
        {
            // Setup
            var dbTypeName = "smallmoney";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(decimal), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForSqlVariant()
        {
            // Setup
            var dbTypeName = "sql_variant";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(SqlVariant), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForText()
        {
            // Setup
            var dbTypeName = "text";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var dbTypeName = "time";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(TimeSpan), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var dbTypeName = "timestamp";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var dbTypeName = "tinyint";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForUniqueIdentifier()
        {
            // Setup
            var dbTypeName = "uniqueidentifier";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(Guid), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var dbTypeName = "varbinary";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(byte[]), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var dbTypeName = "varchar";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod]
        public void TestSqlServerTypeNameToClientTypeResolverForXml()
        {
            // Setup
            var dbTypeName = "xml";

            // Act
            var clientType = m_resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(string), clientType);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowOnExceptionTestSqlServerTypeNameToClientTypeResolverIfDbTypeNameIsNull()
        {
            // Act
            m_resolver.Resolve(null);
        }
    }
}
