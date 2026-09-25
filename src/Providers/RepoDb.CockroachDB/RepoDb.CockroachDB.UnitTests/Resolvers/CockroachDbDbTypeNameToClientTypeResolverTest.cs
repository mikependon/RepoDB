#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.CockroachDB.UnitTests.Resolvers
{
    [TestClass]
    public class CockroachDbDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInt8()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT8");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForQuotedChar()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("\"CHAR\"");

            // Assert
            Assert.AreEqual(typeof(char), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForArray()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("ARRAY");

            // Assert
            Assert.AreEqual(typeof(Array), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForCharacter()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForCharacterVarying()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHARACTER VARYING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForCitext()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CITEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForName()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NAME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForJsonB()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSONB");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForLTree()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LTREE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegClass()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGCLASS");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegNamespace()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGNAMESPACE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegProc()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROC");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegProcedure()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGPROCEDURE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegRole()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGROLE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBool()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOL");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBitVarying()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT VARYING");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForVarBit()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARBIT");

            // Assert
            Assert.AreEqual(typeof(System.Collections.BitArray), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForByteA()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTEA");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForBytes()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BYTES");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForOid()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OID");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForRegType()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REGTYPE");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

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
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimestampWithoutTimeZone()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITHOUT TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMPTZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE PRECISION");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForFloat8()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT8");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInet()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INET");

            // Assert
            Assert.AreEqual(typeof(System.Net.IPAddress), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInt4()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT4");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInterval()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTERVAL");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimeWithoutTimeZone()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

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
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

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
        public void TestCockroachDbDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForFloat4()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT4");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForInt2()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT2");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMETZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME WITH TIME ZONE");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForGeometry()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOMETRY");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForGeography()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("GEOGRAPHY");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTsQuery()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSQUERY");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForTsVector()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TSVECTOR");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestCockroachDbDbTypeNameToClientTypeResolverForOthers()
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("OTHERS");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
