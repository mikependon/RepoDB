#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToMariaDbTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
