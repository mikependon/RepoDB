#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.Oracle.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToOracleTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("number(10)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("number(19)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("number(5)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("number(3)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("number(1)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("number", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("binary_double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("binary_float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("timestamp with time zone", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("interval day to second", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("raw(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("raw", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("number(10)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("timestamp", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("raw(16)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToOracleTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("clob", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToOracleTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToOracleTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
