using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.PostgreSql;

namespace RepoDb.PostgreSql.BulkOperations.UnitTests.DbSettings
{
    /// <summary>
    /// Covers bulk mapping configuration.
    /// </summary>
    [TestClass]
    public class PostgreSqlBulkOperationsDbSettingBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Uses automatic column mapping by default.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorDefaultsToAutomatic()
        {
            // Act
            var setting = new PostgreSqlBulkOperationsDbSetting();


            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToAutomatic()
        {
            // Setup
            var setting = new PostgreSqlBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.Automatic;


            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrict()
        {
            // Setup
            var setting = new PostgreSqlBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Strict,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Retains an explicitly selected mapping behavior.
        /// </summary>
        [TestMethod]
        public void MappingBehaviorCanBeSetToStrictBypass()
        {
            // Setup
            var setting = new PostgreSqlBulkOperationsDbSetting();


            // Act
            setting.BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.StrictBypass;


            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.StrictBypass,
                setting.BulkColumnMappingsBehavior);
        }

        /// <summary>
        /// Keeps configuration local to each instance.
        /// </summary>
        [TestMethod]
        public void InstancesHaveIndependentMappingBehavior()
        {
            // Setup
            var first = new PostgreSqlBulkOperationsDbSetting();
            var second = new PostgreSqlBulkOperationsDbSetting();


            // Act
            first.BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.Strict;


            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic,
                second.BulkColumnMappingsBehavior);
        }

        #endregion
    }
}