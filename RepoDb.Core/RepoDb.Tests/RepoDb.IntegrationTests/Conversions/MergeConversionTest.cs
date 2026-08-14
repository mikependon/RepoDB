using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.IO;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class MergeConversionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Merge<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge<IdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversionUsingTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge<IdentityTable, double>(table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMergeViaTEntityWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidDataException>(() =>
                    connection.Merge<IdentityTable, Guid>(table));
            }
        }

        #endregion

        #region Merge (TableName)<TResult>

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameTypedResultAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Merge<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMergeViaTableNameWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidDataException>(() =>
                    connection.Merge<Guid>(ClassMappedNameCache.Get<IdentityTable>(),
                        table));
            }
        }

        #endregion
    }
}
