using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.DuckDb;

namespace RepoDb.DuckDb.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class DuckDbBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new DuckDbBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(DuckDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new DuckDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = DuckDbBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(DuckDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new DuckDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = DuckDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(DuckDbBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new DuckDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = DuckDbBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(DuckDbBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new DuckDbBulkOperationsDbSetting();
            var second = new DuckDbBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = DuckDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(DuckDbBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}