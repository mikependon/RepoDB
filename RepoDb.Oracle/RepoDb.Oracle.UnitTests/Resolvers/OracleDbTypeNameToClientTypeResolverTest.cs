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
        public void ThrowExceptionOnOracleDbTypeNameToClientTypeResolverIfTheDbTypeIsNull()
        {
            Assert.Throws<NullReferenceException>(() => m_resolver.Resolve(null));
        }
    }
}
