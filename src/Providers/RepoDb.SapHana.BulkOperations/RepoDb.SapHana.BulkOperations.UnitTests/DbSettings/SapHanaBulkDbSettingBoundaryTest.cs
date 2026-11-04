using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SapHana;

namespace RepoDb.SapHana.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class SapHanaBulkDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new SapHanaBulkDbSetting();


            // Assert
            Assert.AreEqual(SapHanaBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = SapHanaBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(SapHanaBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = SapHanaBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(SapHanaBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = SapHanaBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(SapHanaBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new SapHanaBulkDbSetting();
            var second = new SapHanaBulkDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = SapHanaBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(SapHanaBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}