#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToColumnNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForSmallInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("smallint");

            // Assert
            Assert.AreEqual("SMALLINT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("INTEGER", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("int");

            // Assert
            Assert.AreEqual("INTEGER", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBigInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("bigint");

            // Assert
            Assert.AreEqual("BIGINT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("BOOLEAN", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForFloat()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual("FLOAT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDate()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("date");

            // Assert
            Assert.AreEqual("DATE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTime()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time");

            // Assert
            Assert.AreEqual("TIME", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time_tz");

            // Assert
            Assert.AreEqual("TIME WITH TIME ZONE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestamp()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp");

            // Assert
            Assert.AreEqual("TIMESTAMP", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp_tz");

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNumeric()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("numeric");

            // Assert
            Assert.AreEqual("NUMERIC", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDecimal()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("decimal");

            // Assert
            Assert.AreEqual("DECIMAL", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec16()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec16");

            // Assert
            Assert.AreEqual("DECFLOAT(16)", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec34()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec34");

            // Assert
            Assert.AreEqual("DECFLOAT(34)", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInt128()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("int128");

            // Assert
            Assert.AreEqual("INT128", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForChar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("char");

            // Assert
            Assert.AreEqual("CHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarchar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("varchar");

            // Assert
            Assert.AreEqual("VARCHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("binary");

            // Assert
            Assert.AreEqual("BINARY", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarbinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("varbinary");

            // Assert
            Assert.AreEqual("VARBINARY", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForUuid()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual("UUID", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForUnrecognizedTypeName()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("some_unknown_type");

            // Assert
            Assert.AreEqual("LONG VARCHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNull()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve(null);

            // Assert
            Assert.AreEqual("LONG VARCHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual("INTEGER", result, System.StringComparer.Ordinal);
        }
    }
}
