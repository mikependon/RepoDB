#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.DuckDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToDuckDbTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToDuckDbTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
