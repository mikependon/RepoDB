#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToVerticaTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("long varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("long varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("long varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToVerticaTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
