#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.EnterpriseDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToEnterpriseDbStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
