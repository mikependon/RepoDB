#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToVerticaTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("numeric", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp with timezone", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("long varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToVerticaTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("long varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToVerticaTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToVerticaTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
