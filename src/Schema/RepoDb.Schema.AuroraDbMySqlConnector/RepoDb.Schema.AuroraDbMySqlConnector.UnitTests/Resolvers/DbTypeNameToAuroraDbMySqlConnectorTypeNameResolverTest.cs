#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToAuroraDbMySqlConnectorTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToAuroraDbMySqlConnectorTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
