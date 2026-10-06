#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Firebird.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToFirebirdTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("blob_text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double precision", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double precision", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("blob_binary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToFirebirdTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
