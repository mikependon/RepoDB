using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    [TestClass]
    public class Db2DbTypeNameToClientTypeResolverTest
    {
        private readonly Db2DbTypeNameToClientTypeResolver m_resolver = new();

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForNumber()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("NUMBER"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForVarchar2()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARCHAR2"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForNVarchar2()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NVARCHAR2"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDate()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("DATE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTimestamp()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP(6)"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            Assert.AreEqual(typeof(DateTimeOffset), m_resolver.Resolve("TIMESTAMP(6) WITH TIME ZONE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTimestampWithLocalTimeZone()
        {
            Assert.AreEqual(typeof(DateTimeOffset), m_resolver.Resolve("TIMESTAMP(6) WITH LOCAL TIME ZONE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForIntervalDayToSecond()
        {
            Assert.AreEqual(typeof(TimeSpan), m_resolver.Resolve("INTERVAL DAY(2) TO SECOND(6)"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBlob()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForRaw()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("RAW"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBinaryFloat()
        {
            Assert.AreEqual(typeof(float), m_resolver.Resolve("BINARY_FLOAT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBinaryDouble()
        {
            Assert.AreEqual(typeof(double), m_resolver.Resolve("BINARY_DOUBLE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForNClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NCLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForChar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForNChar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("NCHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForVarchar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARCHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForLong()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("LONG"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForRowId()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("ROWID"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForURowId()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("UROWID"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForXmlType()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("XMLTYPE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForFloat()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("FLOAT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDec()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DEC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDecimal()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DECIMAL"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForNumeric()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("NUMERIC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForLongRaw()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("LONG RAW"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBFile()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BFILE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBoolean()
        {
            Assert.AreEqual(typeof(bool), m_resolver.Resolve("BOOLEAN"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBareTimestampWithNoScale()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForIntervalYearToMonth()
        {
            Assert.AreEqual(typeof(TimeSpan), m_resolver.Resolve("INTERVAL YEAR(2) TO MONTH"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverFallsBackToObjectForAnUnmappedType()
        {
            Assert.AreEqual(typeof(object), m_resolver.Resolve("SOME_UNKNOWN_TYPE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverIsCaseInsensitiveAndTrimsWhitespace()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("  varchar2  "));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2DbTypeNameToClientTypeResolverIfTheDbTypeIsNull()
        {
            Assert.Throws<NullReferenceException>(() => m_resolver.Resolve(null));
        }
    }
}
