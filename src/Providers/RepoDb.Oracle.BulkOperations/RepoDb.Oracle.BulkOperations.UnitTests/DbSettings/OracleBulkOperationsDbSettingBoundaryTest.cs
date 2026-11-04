using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.Oracle;

namespace RepoDb.Oracle.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class OracleBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new OracleBulkOperationsDbSetting();


            // Assert
            Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new OracleBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = OracleBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new OracleBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = OracleBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(OracleBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new OracleBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = OracleBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(OracleBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new OracleBulkOperationsDbSetting();
            var second = new OracleBulkOperationsDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = OracleBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}