#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.ClickHouse.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToClickHouseTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("Bool", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("Bool", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("Int32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("Int32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("Int64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("Int16", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("Float64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("Float64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("Float32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("UUID", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("UUID", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("Int32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("DateTime64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToClickHouseTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
