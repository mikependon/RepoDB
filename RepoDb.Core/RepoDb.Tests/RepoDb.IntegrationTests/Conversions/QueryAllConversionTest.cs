using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class QueryAllConversionTest
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

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>().ToList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                foreach (var table in tables)
                {
                    var match = result.First(r => r.Id == table.Id);
                    Assert.AreEqual(table.ColumnInt.ToString(), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>();

                // Assert
                Assert.IsFalse(result.Any());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversionUsingExplicitTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>()).ToList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                foreach (var table in tables)
                {
                    var match = result.First(r => r.Id == table.Id);
                    Assert.AreEqual(table.ColumnInt.ToString(), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionQueryAllViaTEntityWithAutomaticConversionOnIncompatibleType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<InvalidOperationException>(() =>
                    connection.QueryAll<IdentityTableWithRowGuidAsInt>().ToList());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
