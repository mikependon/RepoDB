#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.SapHana.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToSapHanaTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("nclob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("varbinary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("varbinary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSapHanaTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
