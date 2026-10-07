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

namespace RepoDb.Turso.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToTursoStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DECIMAL", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToTursoStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToTursoStringNameResolver();

            // Assert
            Assert.AreEqual("TIME", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
