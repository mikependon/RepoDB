#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Ahtola.UnitTests.Resolvers
{
    [TestClass]
    public class AhtolaDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAhtola();
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestAhtolaDbTypeNameToClientTypeResolverForOther()
        {
            // Setup
            var resolver = new AhtolaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("WHATEVER");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
