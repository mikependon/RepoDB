#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Db2.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToDb2TypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("clob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToDb2TypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
