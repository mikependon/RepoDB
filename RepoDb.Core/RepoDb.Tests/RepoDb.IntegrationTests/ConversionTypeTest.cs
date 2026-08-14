using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests
{
    [TestClass]
    public class ConversionTypeTest
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

        #region Min

        [TestMethod]
        public void TestSqlConnectionMinTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.AreEqual(default(int), result);

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
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field(nameof(IdentityTable.ColumnInt)), (object)null);

                // Assert
                Assert.AreEqual(default(int), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        //[TestMethod]
        //public void TestSqlConnectionMinTResultAutomaticConversionOnNoRows()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionString))
        //    {
        //        // Act
        //        var result = connection.Min<IdentityTable>(e => e.ColumnInt, (object)null);

        //        // Assert
        //        Assert.AreEqual(default(int), result);
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMinStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.Throws<InvalidDataException>(() =>
                    connection.Min<IdentityTable>(e => e.ColumnInt, (object)null));
            }
        }

        #endregion
    }
}
