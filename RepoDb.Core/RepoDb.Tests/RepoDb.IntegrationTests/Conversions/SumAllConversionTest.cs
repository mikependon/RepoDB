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
    public class SumAllConversionTest
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

        #region SumAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionSumAllViaTEntityAutomaticConversion()
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
                var result = connection.SumAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.SumAll<IdentityTable, int>(new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(default, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTEntityAutomaticConversionOnDifferentReturnType()
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
                var result = connection.SumAll<IdentityTable, double>(new Field("ColumnInt"));

                // Assert
                Assert.AreEqual((double)tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionSumAllViaTEntityWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.SumAll<IdentityTable>(e => e.ColumnInt));
            }
        }

        #endregion

        #region SumAll<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultAutomaticConversion()
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
                var result = connection.SumAll<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.SumAll<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.SumAll<IdentityTable, double>(new Field("ColumnInt"));

                // Assert
                Assert.AreEqual((double)tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionSumAllTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.SumAll<IdentityTable, int>(new Field("ColumnInt")));
            }
        }

        #endregion

        #region SumAll (TableName)

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameAutomaticConversion()
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
                var result = connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.SumAll<int>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(default, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameAutomaticConversionOnDifferentReturnType()
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
                var result = connection.SumAll<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual((double)tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionSumAllViaTableNameWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt")));
            }
        }

        #endregion

        #region SumAll<TResult> (TableName)

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameTypedResultAutomaticConversion()
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
                var result = connection.SumAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.SumAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.SumAll<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual((double)tables.Sum(t => t.ColumnInt), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionSumAllViaTableNameTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.SumAll<int>(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt")));
            }
        }

        #endregion
    }
}
