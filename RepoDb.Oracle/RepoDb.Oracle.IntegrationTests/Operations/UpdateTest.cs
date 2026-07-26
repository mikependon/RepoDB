using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateTest
    {
        private static readonly Random m_random = new();

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

        // Local, self-contained mutator - this project's shared Helper (which this workstream does not
        // own) has no "update the properties of an existing entity" equivalent. Deliberately sticks to
        // plain numeric/string columns so the post-update Helper.AssertPropertiesEquality round-trip can't
        // trip over the CHAR/NCHAR blank-padding or CLOB/NCLOB/BLOB/XMLTYPE edge cases already documented
        // in Helper.cs.
        private static void UpdateCompleteTableProperties(CompleteTable table)
        {
            table.ColumnVarchar = $"Updated-{m_random.Next(int.MaxValue)}";
            table.ColumnInt = m_random.Next(int.MinValue, int.MaxValue);
            table.ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12);
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestOracleConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #endregion

        #region TableName

        // NOTE: RepoDb.SqlServer.IntegrationTests.Operations.UpdateTest also has an "AsExpandoObject"
        // variant here. Skipped - this project's shared Helper (which this workstream does not own) has
        // no ExpandoObject/dynamic entity source.

        #region Sync

        [TestMethod]
        public void TestOracleConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestOracleConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestOracleConnectionUpdateWithHintsThrows()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Oracle - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Update<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}
