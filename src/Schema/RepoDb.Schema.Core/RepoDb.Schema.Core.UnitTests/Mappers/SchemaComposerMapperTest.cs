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
    public class SchemaComposerMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaComposerMapper.Clear();
        }

        #region Methods

        [TestMethod]
        public void TestSchemaComposerMapperMappingViaGeneric()
        {
            // Setup
            var schemaComposer = new Mock<ISchemaComposer>().Object;
            SchemaComposerMapper.Add<CustomDbConnection>(schemaComposer, false);

            // Act
            var actual = SchemaComposerMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(schemaComposer, actual);
        }

        [TestMethod]
        public void TestSchemaComposerMapperMappingViaConnectionInstance()
        {
            // Setup
            var schemaComposer = new Mock<ISchemaComposer>().Object;
            SchemaComposerMapper.Add<CustomDbConnection>(schemaComposer, false);

            // Act
            var actual = SchemaComposerMapper.Get(new CustomDbConnection());

            // Assert
            Assert.AreSame(schemaComposer, actual);
        }

        [TestMethod]
        public void TestSchemaComposerMapperGetReturnsNullIfNotMapped()
        {
            // Act
            var actual = SchemaComposerMapper.Get<CustomDbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void ThrowExceptionOnSchemaComposerMapperIfTheMappingAlreadyExistsAndNotForced()
        {
            // Setup
            SchemaComposerMapper.Add<CustomDbConnection>(new Mock<ISchemaComposer>().Object, false);

            // Act/Assert
            Assert.Throws<MappingExistsException>(() =>
                SchemaComposerMapper.Add<CustomDbConnection>(new Mock<ISchemaComposer>().Object, false));
        }

        [TestMethod]
        public void TestSchemaComposerMapperExistingMappingCanBeOverwrittenIfForced()
        {
            // Setup
            var first = new Mock<ISchemaComposer>().Object;
            var second = new Mock<ISchemaComposer>().Object;
            SchemaComposerMapper.Add<CustomDbConnection>(first, false);

            // Act
            SchemaComposerMapper.Add<CustomDbConnection>(second, true);
            var actual = SchemaComposerMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(second, actual);
        }

        [TestMethod]
        public void TestSchemaComposerMapperMappingCanBeRemovedViaGeneric()
        {
            // Setup
            SchemaComposerMapper.Add<CustomDbConnection>(new Mock<ISchemaComposer>().Object, false);

            // Act
            SchemaComposerMapper.Remove<CustomDbConnection>();

            // Assert
            var actual = SchemaComposerMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestSchemaComposerMapperMappingsCanBeCleared()
        {
            // Setup
            SchemaComposerMapper.Add<CustomDbConnection>(new Mock<ISchemaComposer>().Object, false);

            // Act
            SchemaComposerMapper.Clear();

            // Assert
            var actual = SchemaComposerMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        #endregion
    }
}
