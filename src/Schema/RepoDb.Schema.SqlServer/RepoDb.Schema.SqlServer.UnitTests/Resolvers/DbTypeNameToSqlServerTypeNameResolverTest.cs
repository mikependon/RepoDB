#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.SqlServer.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToSqlServerTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForText()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("text");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForClob()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("clob");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForLongText()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("longtext");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("string");

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForBool()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bool");

            // Assert
            Assert.AreEqual("bit", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("bit", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForInt4()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int4");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForInt8()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int8");

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForInt2()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("int2");

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForFloat8()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float8");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForFloat4()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("float4");

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("uniqueidentifier", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForByteA()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("bytea");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForBlob()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("blob");

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForCharacterVarying()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character varying");

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverForCharacter()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("character");

            // Assert
            Assert.AreEqual("char", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual("uniqueidentifier", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverTrimsTheName()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("  Int4  ");

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverReturnsTheUnknownNameAsLowerCase()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("DateTime2");

            // Assert
            Assert.AreEqual("datetime2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverReturnsNullIfTheNameIsNull()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverReturnsNullIfTheNameIsEmpty()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestDbTypeNameToSqlServerTypeNameResolverReturnsNullIfTheNameIsWhiteSpace()
        {
            // Setup
            var resolver = new DbTypeNameToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve("   ");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
