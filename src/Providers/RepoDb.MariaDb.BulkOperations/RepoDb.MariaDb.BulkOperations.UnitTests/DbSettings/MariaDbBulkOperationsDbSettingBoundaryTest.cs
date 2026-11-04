using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.MariaDb;

namespace RepoDb.MariaDb.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class MariaDbBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new MariaDbBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new MariaDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MariaDbBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new MariaDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MariaDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new MariaDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = MariaDbBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new MariaDbBulkOperationsDbSetting();
            var second = new MariaDbBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = MariaDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}