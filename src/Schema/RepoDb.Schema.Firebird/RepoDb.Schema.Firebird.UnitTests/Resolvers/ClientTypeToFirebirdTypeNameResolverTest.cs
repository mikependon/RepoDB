#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Firebird.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToFirebirdTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double precision", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp_tz", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("blob_binary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToFirebirdTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("blob_text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToFirebirdTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToFirebirdTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
