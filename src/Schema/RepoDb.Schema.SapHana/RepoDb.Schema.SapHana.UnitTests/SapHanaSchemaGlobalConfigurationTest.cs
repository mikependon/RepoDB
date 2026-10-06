#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.SapHana.UnitTests
{
    [TestClass]
    public class SapHanaSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseSapHanaSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var actual = SchemaComposerMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<SapHanaSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var first = SchemaComposerMapper.Get<HanaConnection>();
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var second = SchemaComposerMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<SapHanaSchemaComposer>(second);
        }

        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var actual = SchemaReaderMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
