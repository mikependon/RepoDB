#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToEnterpriseDbStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time));
        }
    }
}
