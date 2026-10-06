#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToMariaDbTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("varchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint unsigned", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("boolean", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("double", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uuid", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToMariaDbTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("longtext", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToMariaDbTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToMariaDbTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
