#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToAuroraDbMySqlConnectorTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint unsigned", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToAuroraDbMySqlConnectorTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToAuroraDbMySqlConnectorTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbMySqlConnectorTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
