using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.MySqlConnector;

namespace RepoDb.MySqlConnector.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class MySqlConnectorBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new MySqlConnectorBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(MySqlConnectorBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new MySqlConnectorBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlConnectorBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(MySqlConnectorBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new MySqlConnectorBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlConnectorBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MySqlConnectorBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new MySqlConnectorBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlConnectorBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(MySqlConnectorBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new MySqlConnectorBulkOperationsDbSetting();
            var second = new MySqlConnectorBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = MySqlConnectorBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MySqlConnectorBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}