#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.CockroachDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToCockroachDbStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("INT4", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToCockroachDbStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToCockroachDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
