#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class AuroraDbDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInt8()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT8");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForQuotedChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("\"CHAR\"");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForArray()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("ARRAY");

            // Assert
            Assert.AreEqual(typeof(Array), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForCharacter()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForCharacterVarying()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER VARYING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForCitext()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CITEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForName()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NAME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForJsonB()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONB");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForLTree()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LTREE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegClass()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCLASS");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegNamespace()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGNAMESPACE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegProc()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROC");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegProcedure()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROCEDURE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegRole()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGROLE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBool()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOL");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBitVarying()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT VARYING");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForVarBit()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBIT");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForByteA()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTEA");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForBytes()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTES");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForOid()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForRegType()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGTYPE");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
#if NET6_0_OR_GREATER
            Assert.AreEqual(typeof(DateOnly), result);
#else
            Assert.AreEqual(typeof(DateTime), result);
#endif
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimestampWithoutTimeZone()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITHOUT TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMPTZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE PRECISION");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForFloat8()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT8");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInet()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INET");

            // Assert
            Assert.AreEqual(typeof(System.Net.IPAddress), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInt4()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT4");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInterval()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTERVAL");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimeWithoutTimeZone()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITHOUT TIME ZONE");

            // Assert
#if NET6_0_OR_GREATER
            Assert.AreEqual(typeof(TimeOnly), result);
#else
            Assert.AreEqual(typeof(TimeSpan), result);
#endif
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

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
        public void TestAuroraDbDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForFloat4()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT4");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMETZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForGeography()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOGRAPHY");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTsQuery()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSQUERY");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTsQuery), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForTsVector()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSVECTOR");

            // Assert
            Assert.AreEqual(typeof(NpgsqlTsVector), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForOthers()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OTHERS");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
            #region PostgreSQL Types

        [TestMethod]
        [DataRow("MONEY", typeof(decimal))]
        [DataRow("XML", typeof(string))]
        [DataRow("JSONPATH", typeof(string))]
        [DataRow("XID", typeof(uint))]
        [DataRow("CID", typeof(uint))]
        [DataRow("CIDR", typeof(NpgsqlCidr))]
        [DataRow("MACADDR", typeof(PhysicalAddress))]
        [DataRow("MACADDR8", typeof(PhysicalAddress))]
        [DataRow("BOX", typeof(NpgsqlBox))]
        [DataRow("CIRCLE", typeof(NpgsqlCircle))]
        [DataRow("LINE", typeof(NpgsqlLine))]
        [DataRow("LSEG", typeof(NpgsqlLSeg))]
        [DataRow("PATH", typeof(NpgsqlPath))]
        [DataRow("POINT", typeof(NpgsqlPoint))]
        [DataRow("POLYGON", typeof(NpgsqlPolygon))]
        [DataRow("INT4RANGE", typeof(NpgsqlRange<int>))]
        [DataRow("INT8RANGE", typeof(NpgsqlRange<long>))]
        [DataRow("NUMRANGE", typeof(NpgsqlRange<decimal>))]
        [DataRow("TSRANGE", typeof(NpgsqlRange<DateTime>))]
        [DataRow("TSTZRANGE", typeof(NpgsqlRange<DateTime>))]
        [DataRow("DATERANGE", typeof(NpgsqlRange<DateTime>))]
        [DataRow("HSTORE", typeof(Dictionary<string, string>))]
        [DataRow("PG_LSN", typeof(NpgsqlLogSequenceNumber))]
        [DataRow("TID", typeof(NpgsqlTid))]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForPostgreSqlTypes(string dbTypeName,
            Type expectedType)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(expectedType, result);
        }

        [TestMethod]
        [DataRow("GEOMETRY")]
        [DataRow("GEOGRAPHY")]
        [DataRow("USER-DEFINED")]
        [DataRow("")]
        public void TestAuroraDbDbTypeNameToClientTypeResolverForUnmappedTypes(string dbTypeName)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbDbTypeNameToClientTypeResolverIfTheNameIsNull()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
