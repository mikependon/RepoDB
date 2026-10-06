#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.DuckDb.UnitTests
{
    [TestClass]
    public class DuckDbSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseDuckDbSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var actual = SchemaComposerMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<DuckDbSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var first = SchemaComposerMapper.Get<DuckDBConnection>();
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var second = SchemaComposerMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<DuckDbSchemaComposer>(second);
        }

        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var actual = SchemaReaderMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
