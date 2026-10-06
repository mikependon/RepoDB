#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MariaDbConnector.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToMariaDbConnectorTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint unsigned", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbConnectorTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToMariaDbConnectorTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbConnectorTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
