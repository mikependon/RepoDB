using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    [TestClass]
    public class OracleDbTypeNameToClientTypeResolverTest
    {
        private readonly OracleDbTypeNameToClientTypeResolver m_resolver = new();

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForNumber()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("NUMBER"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForVarchar2()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARCHAR2"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForNVarchar2()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NVARCHAR2"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForDate()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("DATE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForTimestamp()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP(6)"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            Assert.AreEqual(typeof(DateTimeOffset), m_resolver.Resolve("TIMESTAMP(6) WITH TIME ZONE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForTimestampWithLocalTimeZone()
        {
            Assert.AreEqual(typeof(DateTimeOffset), m_resolver.Resolve("TIMESTAMP(6) WITH LOCAL TIME ZONE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForIntervalDayToSecond()
        {
            Assert.AreEqual(typeof(TimeSpan), m_resolver.Resolve("INTERVAL DAY(2) TO SECOND(6)"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBlob()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BLOB"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForRaw()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("RAW"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBinaryFloat()
        {
            Assert.AreEqual(typeof(float), m_resolver.Resolve("BINARY_FLOAT"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBinaryDouble()
        {
            Assert.AreEqual(typeof(double), m_resolver.Resolve("BINARY_DOUBLE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CLOB"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForNClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NCLOB"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForChar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CHAR"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForNChar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NCHAR"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForVarchar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARCHAR"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForLong()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("LONG"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForRowId()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("ROWID"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForURowId()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("UROWID"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForXmlType()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("XMLTYPE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForFloat()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("FLOAT"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForDec()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DEC"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForDecimal()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DECIMAL"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForNumeric()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("NUMERIC"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForLongRaw()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("LONG RAW"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBFile()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BFILE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBoolean()
        {
            Assert.AreEqual(typeof(bool), m_resolver.Resolve("BOOLEAN"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForBareTimestampWithNoScale()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverForIntervalYearToMonth()
        {
            Assert.AreEqual(typeof(TimeSpan), m_resolver.Resolve("INTERVAL YEAR(2) TO MONTH"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverFallsBackToObjectForAnUnmappedType()
        {
            Assert.AreEqual(typeof(object), m_resolver.Resolve("SOME_UNKNOWN_TYPE"));
        }

        [TestMethod]
        public void TestOracleDbTypeNameToClientTypeResolverIsCaseInsensitiveAndTrimsWhitespace()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("  varchar2  "));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleDbTypeNameToClientTypeResolverIfTheDbTypeIsNull()
        {
            Assert.Throws<NullReferenceException>(() => m_resolver.Resolve(null));
        }
    }
}
