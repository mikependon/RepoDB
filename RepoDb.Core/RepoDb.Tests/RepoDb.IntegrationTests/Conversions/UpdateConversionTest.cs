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
    public class UpdateConversionTest
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

        #region Update<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromStringToInt()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "123"
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(123, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromNullStringToInt()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = null
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.IsNull(result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromStringToIntUsingTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "456"
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(456, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionUpdateViaTEntityWithAutomaticConversionOnNonNumericString()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "not-a-number"
                };

                // Assert
                Assert.Throws<FormatException>(() =>
                    connection.Update<IdentityTableWithColumnIntAsString>(updates,
                        fields: Field.From(nameof(IdentityTable.ColumnInt))));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
