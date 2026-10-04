#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.SqlServer.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToSqlServerTypeNameResolverTest
    {
        #region Methods

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForString()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(string));

            // Assert
            Assert.AreEqual("nvarchar", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForInt32()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForInt64()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(long));

            // Assert
            Assert.AreEqual("bigint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForInt16()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(short));

            // Assert
            Assert.AreEqual("smallint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForByte()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte));

            // Assert
            Assert.AreEqual("tinyint", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForBoolean()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(bool));

            // Assert
            Assert.AreEqual("bit", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForDecimal()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(decimal));

            // Assert
            Assert.AreEqual("decimal", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForDouble()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(double));

            // Assert
            Assert.AreEqual("float", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForSingle()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(float));

            // Assert
            Assert.AreEqual("real", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime));

            // Assert
            Assert.AreEqual("datetime2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual("datetimeoffset", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForTimeSpan()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(TimeSpan));

            // Assert
            Assert.AreEqual("time", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForGuid()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid));

            // Assert
            Assert.AreEqual("uniqueidentifier", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForByteArray()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(byte[]));

            // Assert
            Assert.AreEqual("varbinary", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForNullableInt32()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(int?));

            // Assert
            Assert.AreEqual("int", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForNullableDateTime()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(DateTime?));

            // Assert
            Assert.AreEqual("datetime2", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForNullableGuid()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(Guid?));

            // Assert
            Assert.AreEqual("uniqueidentifier", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClientTypeToSqlServerTypeNameResolverForUnknownType()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act
            var actual = resolver.Resolve(typeof(object));

            // Assert
            Assert.AreEqual("sql_variant", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToSqlServerTypeNameResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToSqlServerTypeNameResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        #endregion
    }
}
