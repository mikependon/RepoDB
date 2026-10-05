#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToMySqlConnectorTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint unsigned", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlConnectorTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToMySqlConnectorTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToMySqlConnectorTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
