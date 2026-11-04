using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.CockroachDb;

namespace RepoDb.CockroachDb.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class CockroachDbBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new CockroachDbBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new CockroachDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new CockroachDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new CockroachDbBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new CockroachDbBulkOperationsDbSetting();
            var second = new CockroachDbBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}