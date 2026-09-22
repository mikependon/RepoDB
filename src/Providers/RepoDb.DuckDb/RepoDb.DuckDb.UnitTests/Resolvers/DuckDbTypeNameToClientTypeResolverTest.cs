#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.DuckDb.UnitTests.Resolvers
{
    [TestClass]
    public class DuckDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForTinyInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TINYINT");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SMALLINT");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForHugeInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("HUGEINT");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUTinyInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UTINYINT");

            // Assert
            Assert.AreEqual(typeof(byte), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUSmallInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("USMALLINT");

            // Assert
            Assert.AreEqual(typeof(ushort), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUInteger()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UINTEGER");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUBigInt()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UBIGINT");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FLOAT");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForJson()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("JSON");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMP");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIMESTAMPTZ");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForBit()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForInterval()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTERVAL");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverForUnknown()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("SOME_UNKNOWN_TYPE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestDuckDbTypeNameToClientTypeResolverThrowsOnNull()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }
    }
}
