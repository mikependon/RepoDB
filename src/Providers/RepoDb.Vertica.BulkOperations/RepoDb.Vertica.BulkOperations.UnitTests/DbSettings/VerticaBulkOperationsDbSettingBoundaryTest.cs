using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.Vertica;

namespace RepoDb.Vertica.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class VerticaBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new VerticaBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(VerticaBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new VerticaBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = VerticaBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(VerticaBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new VerticaBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = VerticaBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(VerticaBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new VerticaBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = VerticaBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(VerticaBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new VerticaBulkOperationsDbSetting();
            var second = new VerticaBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = VerticaBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(VerticaBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}