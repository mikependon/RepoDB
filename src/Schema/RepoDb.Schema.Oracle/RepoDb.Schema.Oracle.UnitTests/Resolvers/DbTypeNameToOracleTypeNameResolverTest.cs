#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Oracle.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToOracleTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("varchar2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("clob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("varchar2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("number(1)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("number(1)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("number(10)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("number(10)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("number(19)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("number(5)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("binary_double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("binary_double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("binary_float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("raw(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("raw", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("raw(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("number(10)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToOracleTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
