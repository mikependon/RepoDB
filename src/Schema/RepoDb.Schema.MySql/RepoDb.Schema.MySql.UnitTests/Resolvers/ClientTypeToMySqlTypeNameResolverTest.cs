#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MySql.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToMySqlTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint unsigned", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMySqlTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToMySqlTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToMySqlTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
