using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SqlServer;

namespace RepoDb.SqlServer.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class SqlServerBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new SqlServerBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new SqlServerBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = SqlServerBulkColumnMappingsBehavior.Automatic;

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new SqlServerBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = SqlServerBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new SqlServerBulkOperationsDbSetting();

            // Act
            setting.BulkColumnMappingsBehavior = SqlServerBulkColumnMappingsBehavior.StrictBypass;

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new SqlServerBulkOperationsDbSetting();
            var second = new SqlServerBulkOperationsDbSetting();

            // Act
            first.BulkColumnMappingsBehavior = SqlServerBulkColumnMappingsBehavior.Strict;

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}