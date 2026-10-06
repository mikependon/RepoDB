#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Db2.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToDb2TypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("binary(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDb2TypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("clob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToDb2TypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToDb2TypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
