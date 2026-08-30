#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Resolvers
{
    [TestClass]
    public class EnterpriseDbDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForChar2()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("\"CHAR\"");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForArray()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("ARRAY");

            // Assert
            Assert.AreEqual(typeof(Array), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForCharacter()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForCharacterVarying()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER VARYING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForJsonB()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONB");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForJsonPath()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONPATH");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForName()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NAME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPgDependencies()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_DEPENDENCIES");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPgLsn()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_LSN");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPgMcvList()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_MCV_LIST");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPgNDistinct()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_NDISTINCT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPgNodeTree()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PG_NODE_TREE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRefCursor()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REFCURSOR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegClass()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCLASS");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegDictionary()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGDICTIONARY");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegNamespace()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGNAMESPACE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegOper()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGOPER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegOperator()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGOPERATOR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegProc()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROC");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegProcedure()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROCEDURE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegRole()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGROLE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTxidSnapshot()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TXID_SNAPSHOT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForXml()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("XML");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForBitVarying()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT VARYING");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForBox()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOX");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBBox), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForByteA()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTEA");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForCid()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForOid()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegConfig()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCONFIG");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForRegType()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGTYPE");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForXid()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("XID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForCircle()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CIRCLE");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBCircle), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            // TODO: This requires a mapping whether which type to use.
#if NET6_0_OR_GREATER
            Assert.AreEqual(typeof(DateOnly), result);
#else
            Assert.AreEqual(typeof(DateTime), result);
#endif

        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimestampWithoutTimeZone()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITHOUT TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMPTZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE PRECISION");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForInet()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INET");

            // Assert
            Assert.AreEqual(typeof(System.Net.IPAddress), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForInterval()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTERVAL");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimeWithoutTimeZone()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITHOUT TIME ZONE");

            // Assert
            // TODO: We should not just change it this
#if NET6_0_OR_GREATER
            Assert.AreEqual(typeof(TimeOnly), result);
#else
            Assert.AreEqual(typeof(TimeSpan), result);
#endif
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");
            // Assert
#if NET6_0_OR_GREATER
            Assert.AreEqual(typeof(TimeOnly), result);
#else
            Assert.AreEqual(typeof(TimeSpan), result);
#endif
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForLine()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LINE");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBLine), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForLSeg()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LSEG");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBLSeg), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForMacAddr()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MACADDR");

            // Assert
            Assert.AreEqual(typeof(System.Net.NetworkInformation.PhysicalAddress), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForMacAddr8()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MACADDR8");

            // Assert
            Assert.AreEqual(typeof(System.Net.NetworkInformation.PhysicalAddress), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForMoney()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("MONEY");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForNumerc()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPath()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("PATH");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBPath), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPoint()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POINT");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBPoint), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForPolygon()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("POLYGON");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBPolygon), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTid()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TID");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBTid), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMETZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTsQuery()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSQUERY");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBTsQuery), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForTsVector()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSVECTOR");

            // Assert
            Assert.AreEqual(typeof(EDBTypes.EDBTsVector), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestEnterpriseDbDbTypeNameToClientTypeResolverForOthers()
        {
            // Setup
            var resolver = new EnterpriseDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OTHERS");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
