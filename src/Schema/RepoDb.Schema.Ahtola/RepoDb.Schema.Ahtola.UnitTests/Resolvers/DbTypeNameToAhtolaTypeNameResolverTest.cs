#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Ahtola.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToSqliteTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSqliteTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
