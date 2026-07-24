using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToOracleStringNameResolverTest
    {
        private readonly DbTypeToOracleStringNameResolver m_resolver = new();

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt64()
        {
            Assert.AreEqual("NUMBER(19)", m_resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt32()
        {
            Assert.AreEqual("NUMBER(10)", m_resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt16()
        {
            Assert.AreEqual("NUMBER(5)", m_resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForByte()
        {
            Assert.AreEqual("NUMBER(3)", m_resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForBoolean()
        {
            Assert.AreEqual("NUMBER(1)", m_resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForString()
        {
            Assert.AreEqual("NVARCHAR2(2000)", m_resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForAnsiString()
        {
            Assert.AreEqual("VARCHAR2(2000)", m_resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDate()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTime()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTime2()
        {
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTimeOffset()
        {
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", m_resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDecimal()
        {
            Assert.AreEqual("NUMBER(18,2)", m_resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDouble()
        {
            Assert.AreEqual("BINARY_DOUBLE", m_resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForSingle()
        {
            Assert.AreEqual("BINARY_FLOAT", m_resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForGuid()
        {
            Assert.AreEqual("RAW(16)", m_resolver.Resolve(DbType.Guid));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForBinary()
        {
            Assert.AreEqual("BLOB", m_resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForXml()
        {
            Assert.AreEqual("XMLTYPE", m_resolver.Resolve(DbType.Xml));
        }
    }
}
