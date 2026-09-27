#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToAuroraDbStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("INT4", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAuroraDbStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToAuroraDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
