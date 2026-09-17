#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToSqLiteStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("BLOB", resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DATETIME", resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DECIMAL", resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE", resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("INT", resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToSqLiteStringNameResolver();

            // Assert
            Assert.AreEqual("TIME", resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }
    }
}
