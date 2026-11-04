using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.MySql;

namespace RepoDb.MySql.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class MySqlBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new MySqlBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(MySqlBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new MySqlBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(MySqlBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new MySqlBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MySqlBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new MySqlBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MySqlBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(MySqlBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new MySqlBulkOperationsDbSetting();
            var second = new MySqlBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = MySqlBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MySqlBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}