#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Turso.UnitTests.Resolvers
{
    [TestClass]
    public class TursoDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseTurso();
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestTursoDbTypeNameToClientTypeResolverForOther()
        {
            // Setup
            var resolver = new TursoDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("WHATEVER");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
