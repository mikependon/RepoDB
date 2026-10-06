#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.DuckDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToDuckDbTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("utinyint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp with time zone", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToDuckDbTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToDuckDbTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToDuckDbTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
