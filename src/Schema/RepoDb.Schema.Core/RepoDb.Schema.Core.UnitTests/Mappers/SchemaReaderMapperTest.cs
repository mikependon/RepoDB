#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests.Mappers
{
    [TestClass]
    public class SchemaReaderMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
        }

        #region Methods

        [TestMethod]
        public void TestSchemaReaderMapperMappingViaGeneric()
        {
            // Setup
            var schemaReader = new Mock<ISchemaReader>().Object;
            SchemaReaderMapper.Add<CustomDbConnection>(schemaReader, false);

            // Act
            var actual = SchemaReaderMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(schemaReader, actual);
        }

        [TestMethod]
        public void TestSchemaReaderMapperMappingViaConnectionInstance()
        {
            // Setup
            var schemaReader = new Mock<ISchemaReader>().Object;
            SchemaReaderMapper.Add<CustomDbConnection>(schemaReader, false);

            // Act
            var actual = SchemaReaderMapper.Get(new CustomDbConnection());

            // Assert
            Assert.AreSame(schemaReader, actual);
        }

        [TestMethod]
        public void TestSchemaReaderMapperGetReturnsNullIfNotMapped()
        {
            // Act
            var actual = SchemaReaderMapper.Get<CustomDbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void ThrowExceptionOnSchemaReaderMapperIfTheMappingAlreadyExistsAndNotForced()
        {
            // Setup
            SchemaReaderMapper.Add<CustomDbConnection>(new Mock<ISchemaReader>().Object, false);

            // Act/Assert
            Assert.Throws<MappingExistsException>(() =>
                SchemaReaderMapper.Add<CustomDbConnection>(new Mock<ISchemaReader>().Object, false));
        }

        [TestMethod]
        public void TestSchemaReaderMapperExistingMappingCanBeOverwrittenIfForced()
        {
            // Setup
            var first = new Mock<ISchemaReader>().Object;
            var second = new Mock<ISchemaReader>().Object;
            SchemaReaderMapper.Add<CustomDbConnection>(first, false);

            // Act
            SchemaReaderMapper.Add<CustomDbConnection>(second, true);
            var actual = SchemaReaderMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(second, actual);
        }

        [TestMethod]
        public void TestSchemaReaderMapperMappingCanBeRemovedViaGeneric()
        {
            // Setup
            SchemaReaderMapper.Add<CustomDbConnection>(new Mock<ISchemaReader>().Object, false);

            // Act
            SchemaReaderMapper.Remove<CustomDbConnection>();

            // Assert
            var actual = SchemaReaderMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestSchemaReaderMapperMappingsCanBeCleared()
        {
            // Setup
            SchemaReaderMapper.Add<CustomDbConnection>(new Mock<ISchemaReader>().Object, false);

            // Act
            SchemaReaderMapper.Clear();

            // Assert
            var actual = SchemaReaderMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        #endregion
    }
}
