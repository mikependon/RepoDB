#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Firebird.UnitTests.Resolvers
{
    [TestClass]
    public class FirebirdDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("smallint");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("bigint");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForInt128()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("int128");

            // Assert
            Assert.AreEqual(typeof(System.Numerics.BigInteger), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("numeric");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("decimal");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForDec16()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("dec16");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForDec34()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("dec34");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("char");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("varchar");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForBlobText()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("blob_text");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForBlobBinary()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("blob_binary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("binary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("varbinary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("date");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("time");

            // Assert
            Assert.AreEqual(typeof(TimeSpan), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("timestamp");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("time_tz");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("timestamp_tz");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestFirebirdDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("none");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdDbTypeNameToClientTypeResolverIfTheDbTypeNameIsNull()
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();

            // Act
            Assert.Throws<NullReferenceException>(() => resolver.Resolve(null));
        }
    }
}
