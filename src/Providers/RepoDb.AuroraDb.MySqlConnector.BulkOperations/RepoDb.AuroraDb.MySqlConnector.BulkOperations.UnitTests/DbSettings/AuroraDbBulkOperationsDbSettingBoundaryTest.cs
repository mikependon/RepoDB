using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.AuroraDb;

namespace RepoDb.AuroraDb.MySqlConnector.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class AuroraDbBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new AuroraDbBulkOperationsDbSetting();


            // Assert
            Assert.AreEqual(AuroraDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new AuroraDbBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = AuroraDbBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(AuroraDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new AuroraDbBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = AuroraDbBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(AuroraDbBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new AuroraDbBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = AuroraDbBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(AuroraDbBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new AuroraDbBulkOperationsDbSetting();
            var second = new AuroraDbBulkOperationsDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = AuroraDbBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(AuroraDbBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}