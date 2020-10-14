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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForChar2()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("\"CHAR\"");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForArray()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("ARRAY");

            // Assert
            Assert.AreEqual(typeof(Array), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForCharacter()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForCharacterVarying()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER VARYING");

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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForJsonB()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONB");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForJsonPath()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONPATH");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForName()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NAME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPgDependencies()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_DEPENDENCIES");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPgLsn()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_LSN");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPgMcvList()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_MCV_LIST");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPgNDistinct()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_NDISTINCT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPgNodeTree()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_NODE_TREE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRefCursor()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REFCURSOR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegClass()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCLASS");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegDictionary()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGDICTIONARY");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegNamespace()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGNAMESPACE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegOper()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGOPER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegOperator()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGOPERATOR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegProc()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROC");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegProcedure()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROCEDURE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegRole()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGROLE");

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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTxidSnapshot()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TXID_SNAPSHOT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForXml()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("XML");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(bool), result);
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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBitVarying()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT VARYING");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForBox()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOX");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlBox), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForByteA()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTEA");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForCid()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForOid()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegConfig()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCONFIG");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForRegType()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGTYPE");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForXid()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("XID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForCircle()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CIRCLE");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlCircle), result);
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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimestampWithoutTimeZone()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITHOUT TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }
        
        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }
        
        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }
        
        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMPTZ");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE PRECISION");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInet()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INET");

            // Assert
            Assert.AreEqual(typeof(System.Net.IPAddress), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForInterval()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTERVAL");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimeWithoutTimeZone()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITHOUT TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForLine()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINE");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlLine), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForLSeg()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LSEG");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlLSeg), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMacAddr()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MACADDR");

            // Assert
            Assert.AreEqual(typeof(System.Net.NetworkInformation.PhysicalAddress), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMacAddr8()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MACADDR8");

            // Assert
            Assert.AreEqual(typeof(System.Net.NetworkInformation.PhysicalAddress), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForMoney()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MONEY");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForNumerc()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPath()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PATH");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlPath), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlPoint), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlPolygon), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(float), result);
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
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTid()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TID");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlTid), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMETZ");

            // Assert
            Assert.AreEqual(typeof(System.DateTimeOffset), result);
        }
        
        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(System.DateTimeOffset), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTsQuery()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSQUERY");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlTsQuery), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForTsVector()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSVECTOR");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTypes.NpgsqlTsVector), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestPostgreSqlDbTypeNameToClientTypeResolverForOthers()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OTHERS");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
