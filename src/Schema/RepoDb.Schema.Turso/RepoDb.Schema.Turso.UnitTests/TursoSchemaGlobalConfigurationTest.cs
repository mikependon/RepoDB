#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Turso.UnitTests
{
    [TestClass]
    public class TursoSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseTursoSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var actual = SchemaComposerMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<TursoSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var first = SchemaComposerMapper.Get<SqliteConnection>();
            GlobalConfiguration.Setup().UseTursoSchema();
            var second = SchemaComposerMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<TursoSchemaComposer>(second);
        }

        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var actual = SchemaReaderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
