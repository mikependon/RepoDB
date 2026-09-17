#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToPostgreSqlStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
