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
    public class MinConversionTest
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

        #region Min<TEntity>

        [TestMethod]
        public void TestSqlConnectionMinViaTEntityAutomaticConversion()
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
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Min<IdentityTable, int>(new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(default, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTEntityAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Min<IdentityTable, double>(new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMinViaTEntityWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Min<IdentityTable>(e => e.ColumnInt, (object)null));
            }
        }

        #endregion

        #region Min<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionMinTypedResultAutomaticConversion()
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
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Min<IdentityTable, double>(new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMinTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Min<IdentityTable, int>(new Field("ColumnInt"), (object)null));
            }
        }

        #endregion

        #region Min (TableName)

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameAutomaticConversion()
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
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Min<int>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(default, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Min< double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMinViaTableNameWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt"),
                        (object)null));
            }
        }

        #endregion

        #region Min<TResult> (TableName)

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultAutomaticConversion()
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
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Min<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.First().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMinViaTableNameTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Min<int>(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt"),
                        (object)null));
            }
        }

        #endregion
    }
}
