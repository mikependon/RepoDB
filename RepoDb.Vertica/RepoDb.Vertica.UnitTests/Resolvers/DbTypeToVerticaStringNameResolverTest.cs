#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToVerticaStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForInt64()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForInt32()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int32);

            // Assert
            Assert.AreEqual("INTEGER", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForInt16()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForByte()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Byte);

            // Assert - Vertica has no TINYINT; the next-widest exact type is SMALLINT.
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForDouble()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForSingle()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Single);

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL(18,2)", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Boolean);

            // Assert
            Assert.AreEqual("BOOLEAN", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForDate()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Date);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.DateTime);

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.DateTimeOffset);

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForTime()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Time);

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForGuid()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Guid);

            // Assert
            Assert.AreEqual("CHAR(16) CHARACTER SET OCTETS", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForBinary()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Binary);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE 0", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.String);

            // Assert
            Assert.AreEqual("VARCHAR(8191)", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.AnsiStringFixedLength);

            // Assert
            Assert.AreEqual("CHAR(8191)", result);
        }

        [TestMethod]
        public void TestDbTypeToVerticaStringNameResolverForXml()
        {
            // Setup
            var resolver = new DbTypeToVerticaStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Xml);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result);
        }
    }
}
