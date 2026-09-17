#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Firebird.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToColumnNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForSmallInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("smallint");

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("INTEGER", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBigInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("bigint");

            // Assert
            Assert.AreEqual("BIGINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("BOOLEAN", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForFloat()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual("FLOAT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDate()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("date");

            // Assert
            Assert.AreEqual("DATE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTime()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time");

            // Assert
            Assert.AreEqual("TIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time_tz");

            // Assert
            Assert.AreEqual("TIME WITH TIME ZONE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestamp()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp");

            // Assert
            Assert.AreEqual("TIMESTAMP", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp_tz");

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNumeric()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("numeric");

            // Assert
            Assert.AreEqual("NUMERIC", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDecimal()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("decimal");

            // Assert
            Assert.AreEqual("DECIMAL", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec16()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec16");

            // Assert
            Assert.AreEqual("DECFLOAT(16)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec34()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec34");

            // Assert
            Assert.AreEqual("DECFLOAT(34)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInt128()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("int128");

            // Assert
            Assert.AreEqual("INT128", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForChar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("char");

            // Assert
            Assert.AreEqual("CHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarchar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("varchar");

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - binary maps onto the same base keyword as char; callers append the size and
            // "CHARACTER SET OCTETS" themselves.
            var result = resolver.Resolve("binary");

            // Assert
            Assert.AreEqual("CHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarbinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - varbinary maps onto the same base keyword as varchar; callers append the size and
            // "CHARACTER SET OCTETS" themselves.
            var result = resolver.Resolve("varbinary");

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBlobBinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("blob_binary");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE 0", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBlobText()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("blob_text");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForUnrecognizedTypeName()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - unrecognized type names fall back to the same safe catch-all as "blob_text".
            var result = resolver.Resolve("some_unknown_type");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNull()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve(null);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual("INTEGER", result, StringComparer.Ordinal);
        }
    }
}
