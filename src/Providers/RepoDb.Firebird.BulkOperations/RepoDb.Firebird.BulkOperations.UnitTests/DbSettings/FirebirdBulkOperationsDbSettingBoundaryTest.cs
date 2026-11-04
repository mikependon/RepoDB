using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.Firebird;

namespace RepoDb.Firebird.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class FirebirdBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new FirebirdBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(FirebirdBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new FirebirdBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = FirebirdBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(FirebirdBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new FirebirdBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = FirebirdBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(FirebirdBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new FirebirdBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = FirebirdBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(FirebirdBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new FirebirdBulkOperationsDbSetting();
            var second = new FirebirdBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = FirebirdBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(FirebirdBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}