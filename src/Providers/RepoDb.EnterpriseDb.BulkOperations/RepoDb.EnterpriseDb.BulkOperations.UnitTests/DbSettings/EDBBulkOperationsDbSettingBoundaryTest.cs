using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.EnterpriseDb;

namespace RepoDb.EnterpriseDb.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class EDBBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new EDBBulkOperationsDbSetting();


            // Assert
            Assert.AreEqual(EDBBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new EDBBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = EDBBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(EDBBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new EDBBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = EDBBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(EDBBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new EDBBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = EDBBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(EDBBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new EDBBulkOperationsDbSetting();
            var second = new EDBBulkOperationsDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = EDBBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(EDBBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}