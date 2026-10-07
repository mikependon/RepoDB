#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.ClickHouse.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToClickHouseTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("Int32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("Int64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("Int16", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("UInt8", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("Bool", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("Decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("Float64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("Float32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("DateTime64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("DateTime64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("UUID", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("Int32", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("DateTime64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("UUID", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToClickHouseTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("String", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToClickHouseTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
