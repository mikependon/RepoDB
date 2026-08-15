using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.IO;
using System.Linq;

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
                Assert.Throws<InvalidCastException>(() =>
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
                Assert.Throws<InvalidCastException>(() =>
                    connection.Merge<Guid>(ClassMappedNameCache.Get<IdentityTable>(),
                        table));
            }
        }

        #endregion

        #region Merge<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversionFromStringToInt()
        {
            // Setup
            var table = new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = "123",
                ColumnNVarChar = Guid.NewGuid().ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var id = connection.Merge<IdentityTableWithColumnIntAsString, long>(table);

                // Assert
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(123, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversionFromNullStringToInt()
        {
            // Setup
            var table = new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = null,
                ColumnNVarChar = Guid.NewGuid().ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var id = connection.Merge<IdentityTableWithColumnIntAsString, long>(table);

                // Assert
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.IsNull(result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTEntityAutomaticConversionFromStringToIntUsingTableName()
        {
            // Setup
            var table = new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = "456",
                ColumnNVarChar = Guid.NewGuid().ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var id = connection.Merge<IdentityTableWithColumnIntAsString, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(456, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMergeViaTEntityWithAutomaticConversionOnNonNumericString()
        {
            // Setup
            var table = new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = "not-a-number",
                ColumnNVarChar = Guid.NewGuid().ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<FormatException>(() =>
                    connection.Merge<IdentityTableWithColumnIntAsString, long>(table));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
