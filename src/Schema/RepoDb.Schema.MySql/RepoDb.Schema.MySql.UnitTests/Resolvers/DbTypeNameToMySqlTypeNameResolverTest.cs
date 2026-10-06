#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MySql.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToMySqlTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMySqlTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
