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
    public class InsertConversionTest
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

        #region Insert<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionUsingTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, double>(table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionInsertViaTEntityWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Insert<IdentityTable, Guid>(table));
            }
        }

        #endregion

        #region Insert (TableName)<TResult>

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameTypedResultAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionInsertViaTableNameWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Insert<Guid>(ClassMappedNameCache.Get<IdentityTable>(),
                        table));
            }
        }

        #endregion

        #region Insert<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionFromStringToInt()
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
                var id = connection.Insert<IdentityTableWithColumnIntAsString, long>(table);

                // Assert
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(123, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionFromNullStringToInt()
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
                var id = connection.Insert<IdentityTableWithColumnIntAsString, long>(table);

                // Assert
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.IsNull(result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionFromStringToIntUsingTableName()
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
                var id = connection.Insert<IdentityTableWithColumnIntAsString, long>(ClassMappedNameCache.Get<IdentityTable>(),
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
        public void ThrowExceptionOnSqlConnectionInsertViaTEntityWithAutomaticConversionOnNonNumericString()
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
                    connection.Insert<IdentityTableWithColumnIntAsString, long>(table));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
