using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.Db2;

namespace RepoDb.Db2.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class Db2BulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new Db2BulkOperationsDbSetting();


            // Assert
            Assert.AreEqual(Db2BulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new Db2BulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = Db2BulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(Db2BulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new Db2BulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = Db2BulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(Db2BulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new Db2BulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = Db2BulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(Db2BulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new Db2BulkOperationsDbSetting();
            var second = new Db2BulkOperationsDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = Db2BulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(Db2BulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}