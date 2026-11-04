using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.ClickHouse;

namespace RepoDb.ClickHouse.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class ClickHouseBulkDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new ClickHouseBulkDbSetting();


            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new ClickHouseBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = ClickHouseBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new ClickHouseBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = ClickHouseBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new ClickHouseBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = ClickHouseBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new ClickHouseBulkDbSetting();
            var second = new ClickHouseBulkDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = ClickHouseBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}