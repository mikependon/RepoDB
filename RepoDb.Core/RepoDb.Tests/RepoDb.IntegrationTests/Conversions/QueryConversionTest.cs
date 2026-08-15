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
    public class QueryConversionTest
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

        #region Query<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>(table.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(table.ColumnInt.ToString(), result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>((object)null);

                // Assert
                Assert.IsFalse(result.Any());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversionUsingExplicitTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    table.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(table.ColumnInt.ToString(), result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionQueryViaTEntityWithAutomaticConversionOnIncompatibleType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<InvalidOperationException>(() =>
                    connection.Query<IdentityTableWithRowGuidAsInt>(table.Id).FirstOrDefault());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
