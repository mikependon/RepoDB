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
        public void TestDb2DbTypeNameToClientTypeResolverForSmallInt()
        {
            Assert.AreEqual(typeof(short), m_resolver.Resolve("SMALLINT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForInteger()
        {
            Assert.AreEqual(typeof(int), m_resolver.Resolve("INTEGER"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForInt()
        {
            Assert.AreEqual(typeof(int), m_resolver.Resolve("INT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBigInt()
        {
            Assert.AreEqual(typeof(long), m_resolver.Resolve("BIGINT"));
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
        public void TestDb2DbTypeNameToClientTypeResolverForDec()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DEC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDecFloat()
        {
            Assert.AreEqual(typeof(decimal), m_resolver.Resolve("DECFLOAT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForReal()
        {
            Assert.AreEqual(typeof(float), m_resolver.Resolve("REAL"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDouble()
        {
            Assert.AreEqual(typeof(double), m_resolver.Resolve("DOUBLE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDoublePrecision()
        {
            Assert.AreEqual(typeof(double), m_resolver.Resolve("DOUBLE PRECISION"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForFloat()
        {
            Assert.AreEqual(typeof(double), m_resolver.Resolve("FLOAT"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForChar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForCharacter()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CHARACTER"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForVarchar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARCHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForLongVarchar()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("LONG VARCHAR"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForGraphic()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("GRAPHIC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForVarGraphic()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("VARGRAPHIC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForLongVarGraphic()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("LONG VARGRAPHIC"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("CLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDbClob()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("DBCLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForXml()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("XML"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForRowId()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("ROWID"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForDate()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("DATE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTime()
        {
            Assert.AreEqual(typeof(TimeSpan), m_resolver.Resolve("TIME"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBareTimestampWithNoScale()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTimestampWithScale()
        {
            Assert.AreEqual(typeof(DateTime), m_resolver.Resolve("TIMESTAMP(6)"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            Assert.AreEqual(typeof(DateTimeOffset), m_resolver.Resolve("TIMESTAMP(6) WITH TIME ZONE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBlob()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BLOB"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBinary()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("BINARY"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForVarBinary()
        {
            Assert.AreEqual(typeof(byte[]), m_resolver.Resolve("VARBINARY"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverForBoolean()
        {
            Assert.AreEqual(typeof(bool), m_resolver.Resolve("BOOLEAN"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverFallsBackToObjectForAnUnmappedType()
        {
            Assert.AreEqual(typeof(object), m_resolver.Resolve("SOME_UNKNOWN_TYPE"));
        }

        [TestMethod]
        public void TestDb2DbTypeNameToClientTypeResolverIsCaseInsensitiveAndTrimsWhitespace()
        {
            Assert.AreEqual(typeof(string), m_resolver.Resolve("  varchar  "));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2DbTypeNameToClientTypeResolverIfTheDbTypeIsNull()
        {
            Assert.Throws<NullReferenceException>(() => m_resolver.Resolve(null));
        }
    }
}
