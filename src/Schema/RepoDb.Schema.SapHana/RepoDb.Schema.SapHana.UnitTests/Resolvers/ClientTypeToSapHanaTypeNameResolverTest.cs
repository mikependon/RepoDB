#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.SapHana.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToSapHanaTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("varbinary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("varbinary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSapHanaTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("nclob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToSapHanaTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToSapHanaTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
