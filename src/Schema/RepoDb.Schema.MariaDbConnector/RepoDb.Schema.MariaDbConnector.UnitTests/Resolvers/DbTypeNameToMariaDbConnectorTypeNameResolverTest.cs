#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MariaDbConnector.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToMariaDbConnectorTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToMariaDbConnectorTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
