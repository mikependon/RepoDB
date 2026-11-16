#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Db2.UnitTests
{
    /// <summary>
    /// Contains the db2 schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class Db2SchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies db2 schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseDb2Schema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies db2 schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseDb2Schema();
            var actual = SchemaComposerMapper.Get<DB2Connection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<Db2SchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies db2 schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseDb2Schema();
            var first = SchemaComposerMapper.Get<DB2Connection>();
            GlobalConfiguration.Setup().UseDb2Schema();
            var second = SchemaComposerMapper.Get<DB2Connection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<Db2SchemaComposer>(second);
        }

        /// <summary>
        /// Verifies db2 schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseDb2Schema();
            var actual = SchemaReaderMapper.Get<DB2Connection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
