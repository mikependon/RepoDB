#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests
{
    [TestClass]
    public class AhtolaSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAhtolaSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var actual = SchemaComposerMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<AhtolaSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var first = SchemaComposerMapper.Get<SqliteConnection>();
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var second = SchemaComposerMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<AhtolaSchemaComposer>(second);
        }

        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var actual = SchemaReaderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
