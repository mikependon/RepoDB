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
            Assert.AreEqual("BIGINT", m_resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForInt32()
        {
            Assert.AreEqual("INTEGER", m_resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForInt16()
        {
            Assert.AreEqual("SMALLINT", m_resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForByte()
        {
            Assert.AreEqual("SMALLINT", m_resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForBoolean()
        {
            Assert.AreEqual("SMALLINT", m_resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForString()
        {
            Assert.AreEqual("VARCHAR(2000)", m_resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForAnsiString()
        {
            Assert.AreEqual("VARCHAR(2000)", m_resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDate()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTime()
        {
            // Db2's DATE type has no time-of-day component, unlike Oracle's - a plain DateTime is
            // cast to TIMESTAMP instead so the time portion isn't silently truncated.
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTime2()
        {
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDateTimeOffset()
        {
            // The IBM.Data.Db2 DB2Type enumeration has no timezone-aware member - "TIMESTAMP" is
            // used on a best-effort basis; the offset itself is not preserved.
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDecimal()
        {
            Assert.AreEqual("DECIMAL(31,15)", m_resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForDouble()
        {
            Assert.AreEqual("DOUBLE", m_resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForSingle()
        {
            Assert.AreEqual("REAL", m_resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForGuid()
        {
            // Db2 has no native GUID/UNIQUEIDENTIFIER type; the idiomatic storage for one is a
            // fixed-length 16-byte "CHAR(16) FOR BIT DATA" column.
            Assert.AreEqual("CHAR(16) FOR BIT DATA", m_resolver.Resolve(DbType.Guid));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForBinary()
        {
            Assert.AreEqual("BLOB(1M)", m_resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForXml()
        {
            Assert.AreEqual("XML", m_resolver.Resolve(DbType.Xml));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForAnsiStringFixedLength()
        {
            // Db2 CHAR's maximum length is 254 bytes (unlike Oracle's, whose maximum is 2000).
            Assert.AreEqual("CHAR(254)", m_resolver.Resolve(DbType.AnsiStringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForStringFixedLength()
        {
            // Db2 has no "NCHAR" type; GRAPHIC is its fixed-length double-byte/graphic string type,
            // with a maximum length of 127 characters.
            Assert.AreEqual("GRAPHIC(127)", m_resolver.Resolve(DbType.StringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForObject()
        {
            Assert.AreEqual("BLOB(1M)", m_resolver.Resolve(DbType.Object));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverForTime()
        {
            // Db2's TIME type has no sub-second precision at all - there is no lossless Db2
            // equivalent for a fractional-second duration the way Oracle's INTERVAL DAY TO SECOND
            // provides one.
            Assert.AreEqual("TIME", m_resolver.Resolve(DbType.Time));
        }

        [TestMethod]
        public void TestDbTypeToDb2StringNameResolverFallsBackToVarcharForUnmappedDbTypes()
        {
            // DbType.Currency has no explicit case in the switch, so it should hit the default arm.
            Assert.AreEqual("VARCHAR(2000)", m_resolver.Resolve(DbType.Currency));
        }
    }
}
