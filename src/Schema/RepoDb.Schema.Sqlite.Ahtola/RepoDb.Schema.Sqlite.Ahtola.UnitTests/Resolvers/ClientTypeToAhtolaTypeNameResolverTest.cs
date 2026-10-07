#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToSqliteTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("blob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("integer", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqliteTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("text", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToSqliteTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToSqliteTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
