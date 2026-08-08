using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToDb2StringNameResolverTest
    {
        private readonly DbTypeToDb2StringNameResolver m_resolver = new();

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForInt64()
        {
            Assert.AreEqual("NUMBER(19)", m_resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForInt32()
        {
            Assert.AreEqual("NUMBER(10)", m_resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForInt16()
        {
            Assert.AreEqual("NUMBER(5)", m_resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForByte()
        {
            Assert.AreEqual("NUMBER(3)", m_resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForBoolean()
        {
            Assert.AreEqual("NUMBER(1)", m_resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForString()
        {
            Assert.AreEqual("NVARCHAR2(2000)", m_resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForAnsiString()
        {
            Assert.AreEqual("VARCHAR2(2000)", m_resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDate()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTime()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTime2()
        {
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTimeOffset()
        {
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", m_resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDecimal()
        {
            Assert.AreEqual("NUMBER(18,2)", m_resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDouble()
        {
            Assert.AreEqual("BINARY_DOUBLE", m_resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForSingle()
        {
            Assert.AreEqual("BINARY_FLOAT", m_resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForGuid()
        {
            Assert.AreEqual("RAW(16)", m_resolver.Resolve(DbType.Guid));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForBinary()
        {
            Assert.AreEqual("BLOB", m_resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForXml()
        {
            Assert.AreEqual("XMLTYPE", m_resolver.Resolve(DbType.Xml));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForAnsiStringFixedLength()
        {
            Assert.AreEqual("CHAR(2000)", m_resolver.Resolve(DbType.AnsiStringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForStringFixedLength()
        {
            Assert.AreEqual("NCHAR(2000)", m_resolver.Resolve(DbType.StringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForObject()
        {
            Assert.AreEqual("BLOB", m_resolver.Resolve(DbType.Object));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForTime()
        {
            Assert.AreEqual("INTERVAL DAY(0) TO SECOND(7)", m_resolver.Resolve(DbType.Time));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverFallsBackToNVarchar2ForUnmappedDbTypes()
        {
            // DbType.Currency has no explicit case in the switch, so it should hit the default arm.
            Assert.AreEqual("NVARCHAR2(2000)", m_resolver.Resolve(DbType.Currency));
        }
    }
}
