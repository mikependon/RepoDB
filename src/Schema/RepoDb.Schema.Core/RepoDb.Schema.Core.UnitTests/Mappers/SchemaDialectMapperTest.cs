#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests.Mappers
{
    [TestClass]
    public class SchemaDialectMapperTest
    {
        #region Public Methods

        [TestCleanup]
        public void Cleanup()
        {
            SchemaDialectMapper.Remove("MapperA");
            SchemaDialectMapper.Remove("MapperB");
        }

        [TestMethod]
        public void TestAddMapsTheProviderToTheDialect()
        {
            // Act
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreSame(CustomSchemaDialect.Instance, SchemaDialectMapper.Get("MapperA"));
        }

        [TestMethod]
        public void TestAddReplacesTheDialectOfTheProvider()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);

            // Act
            SchemaDialectMapper.Add("MapperA",
                OtherSchemaDialect.Instance);

            // Assert
            Assert.AreSame(OtherSchemaDialect.Instance, SchemaDialectMapper.Get("MapperA"));
        }

        [TestMethod]
        public void TestAddWithoutAProviderThrows()
        {
            // Act/Assert
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            SchemaDialectMapper.Add(" ", CustomSchemaDialect.Instance));
        }

        [TestMethod]
        public void TestAddWithoutADialectThrows()
        {
            // Act/Assert
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            SchemaDialectMapper.Add("MapperA", null));
        }

        [TestMethod]
        public void TestGetIgnoresTheCaseOfTheProvider()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);

            // Act
            var dialect = SchemaDialectMapper.Get("mappera");

            // Assert
            Assert.AreSame(CustomSchemaDialect.Instance, dialect);
        }

        [TestMethod]
        public void TestGetOfAnUnknownProviderIsNull()
        {
            // Act
            var dialect = SchemaDialectMapper.Get("MapperB");

            // Assert
            Assert.IsNull(dialect);
        }

        [TestMethod]
        public void TestGetOfANullProviderIsNull()
        {
            // Act
            var dialect = SchemaDialectMapper.Get(null);

            // Assert
            Assert.IsNull(dialect);
        }

        [TestMethod]
        public void TestGetAllIncludesTheDialectsThatAreMapped()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);

            // Act
            var dialects = SchemaDialectMapper.GetAll();

            // Assert
            CollectionAssert.Contains(dialects.ToList(), CustomSchemaDialect.Instance);
        }

        [TestMethod]
        public void TestGetAllDoesNotRepeatTheDialectOfManyProviders()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);
            SchemaDialectMapper.Add("MapperB",
                CustomSchemaDialect.Instance);

            // Act
            var dialects = SchemaDialectMapper.GetAll();

            // Assert
            Assert.AreEqual(1, dialects.Count(item => item == CustomSchemaDialect.Instance));
        }

        [TestMethod]
        public void TestGetAllWithADialectAlwaysIncludesIt()
        {
            // Act
            var dialects = SchemaDialectMapper.GetAll(OtherSchemaDialect.Instance);

            // Assert
            CollectionAssert.Contains(dialects.ToList(), OtherSchemaDialect.Instance);
        }

        [TestMethod]
        public void TestGetAllWithADialectDoesNotRepeatIt()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                OtherSchemaDialect.Instance);

            // Act
            var dialects = SchemaDialectMapper.GetAll(OtherSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(1, dialects.Count(item => item == OtherSchemaDialect.Instance));
        }

        [TestMethod]
        public void TestRemoveDeletesTheMapping()
        {
            // Setup
            SchemaDialectMapper.Add("MapperA",
                CustomSchemaDialect.Instance);

            // Act
            SchemaDialectMapper.Remove("MapperA");

            // Assert
            Assert.IsNull(SchemaDialectMapper.Get("MapperA"));
        }

        [TestMethod]
        public void TestRemoveOfANullProviderDoesNothing()
        {
            // Act
            SchemaDialectMapper.Remove(null);

            // Assert
            Assert.IsNull(SchemaDialectMapper.Get(null));
        }

        #endregion
    }
}
