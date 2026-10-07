#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Ahtola.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToAhtolaStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DECIMAL", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToAhtolaStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();

            // Assert
            Assert.AreEqual("TIME", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
