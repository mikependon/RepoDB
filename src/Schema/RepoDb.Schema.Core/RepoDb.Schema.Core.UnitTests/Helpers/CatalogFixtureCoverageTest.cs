using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;
using System;
using System.Data;
using System.Linq;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class CatalogFixtureCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Registers the concrete ADO.NET fixture without runtime proxies.
        /// </summary>
        [TestMethod]
        public void ConnectionRegistersStaticSettings()
        {
            // Act
            using var connection = new CatalogConnection();

            // Assert
            Assert.IsInstanceOfType<CatalogDbSetting>(DbSettingMapper.Get<CatalogConnection>());
        }

        /// <summary>
        /// Reports a closed connection after closing.
        /// </summary>
        [TestMethod]
        public void ConnectionCloses()
        {
            // Setup
            using var connection = new CatalogConnection();

            // Act
            connection.Close();

            // Assert
            Assert.AreEqual(ConnectionState.Closed,
                connection.State);
        }

        /// <summary>
        /// Allows a closed fixture to reopen.
        /// </summary>
        [TestMethod]
        public void ConnectionReopens()
        {
            // Setup
            using var connection = new CatalogConnection();
            connection.Close();

            // Act
            connection.Open();

            // Assert
            Assert.AreEqual(ConnectionState.Open,
                connection.State);
        }

        /// <summary>
        /// Records a requested database change.
        /// </summary>
        [TestMethod]
        public void ConnectionChangesDatabase()
        {
            // Setup
            using var connection = new CatalogConnection();

            // Act
            connection.ChangeDatabase("Other");

            // Assert
            Assert.AreEqual("Other",
                connection.ConnectionString,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Exposes deterministic catalog server metadata.
        /// </summary>
        [TestMethod]
        public void ConnectionExposesStableIdentity()
        {
            // Act
            using var connection = new CatalogConnection();

            // Assert
            Assert.AreEqual("Catalog",
                connection.Database,
                StringComparer.Ordinal);
            Assert.AreEqual("Memory",
                connection.DataSource,
                StringComparer.Ordinal);
            Assert.AreEqual("1.0",
                connection.ServerVersion,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects unsupported transaction creation.
        /// </summary>
        [TestMethod]
        public void ConnectionRejectsTransactions()
        {
            // Setup
            using var connection = new CatalogConnection();

            // Act
            Action action = () => connection.BeginTransaction();

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(action);
        }

        /// <summary>
        /// Lets a query choose its own in-memory result.
        /// </summary>
        [TestMethod]
        public void CommandUsesConfiguredCatalogResults()
        {
            // Setup
            using var connection = new CatalogConnection();
            connection.Data.Columns.Add("Value",
                typeof(int));
            connection.Data.Rows.Add(42);
            connection.Query = sql => connection.Data;
            using var command = connection.CreateCommand();

            // Act
            using var reader = command.ExecuteReader();
            reader.Read();

            // Assert
            Assert.AreEqual(42,
                reader.GetInt32(0));
        }

        /// <summary>
        /// Reads the first column of the first catalog row.
        /// </summary>
        [TestMethod]
        public void CommandReturnsScalarValue()
        {
            // Setup
            using var connection = new CatalogConnection();
            connection.Data.Columns.Add("Value",
                typeof(int));
            connection.Data.Rows.Add(42);
            using var command = connection.CreateCommand();

            // Act
            var actual = command.ExecuteScalar();

            // Assert
            Assert.AreEqual(42,
                actual);
        }

        /// <summary>
        /// Preserves an empty scalar result.
        /// </summary>
        [TestMethod]
        public void CommandReturnsNullForEmptyScalar()
        {
            // Setup
            using var connection = new CatalogConnection();
            using var command = connection.CreateCommand();

            // Act
            var actual = command.ExecuteScalar();

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Keeps fixture control operations free of external effects.
        /// </summary>
        [TestMethod]
        public void CommandSupportsPreparationAndCancellation()
        {
            // Setup
            using var connection = new CatalogConnection();
            using var command = connection.CreateCommand();

            // Act
            command.Prepare();
            command.Cancel();
            var actual = command.ExecuteNonQuery();

            // Assert
            Assert.AreEqual(0,
                actual);
        }

        /// <summary>
        /// Retains command execution settings.
        /// </summary>
        [TestMethod]
        public void CommandPreservesExecutionProperties()
        {
            // Setup
            using var connection = new CatalogConnection();
            using var command = connection.CreateCommand();

            // Act
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 42;
            command.CommandType = CommandType.StoredProcedure;

            // Assert
            Assert.AreEqual("SELECT 1",
                command.CommandText,
                StringComparer.Ordinal);
            Assert.AreEqual(42,
                command.CommandTimeout);
            Assert.AreEqual(CommandType.StoredProcedure,
                command.CommandType);
        }

        /// <summary>
        /// Retains designer and row update settings.
        /// </summary>
        [TestMethod]
        public void CommandPreservesDesignProperties()
        {
            // Setup
            using var connection = new CatalogConnection();
            using var command = connection.CreateCommand();

            // Act
            command.DesignTimeVisible = true;
            command.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            command.Transaction = null;

            // Assert
            Assert.IsTrue(command.DesignTimeVisible);
            Assert.AreEqual(UpdateRowSource.FirstReturnedRecord,
                command.UpdatedRowSource);
            Assert.IsNull(command.Transaction);
        }

        /// <summary>
        /// Restores the default database parameter type.
        /// </summary>
        [TestMethod]
        public void ParameterResetsDatabaseType()
        {
            // Setup
            var parameter = new CatalogParameter { DbType = DbType.Int32 };

            // Act
            parameter.ResetDbType();

            // Assert
            Assert.AreEqual(DbType.String,
                parameter.DbType);
        }

        /// <summary>
        /// Retains source-column metadata.
        /// </summary>
        [TestMethod]
        public void ParameterPreservesColumnMetadata()
        {
            // Setup
            var parameter = new CatalogParameter();

            // Act
            parameter.SourceColumn = "Id";
            parameter.SourceColumnNullMapping = true;
            parameter.Size = 42;

            // Assert
            Assert.AreEqual("Id",
                parameter.SourceColumn,
                StringComparer.Ordinal);
            Assert.IsTrue(parameter.SourceColumnNullMapping);
            Assert.AreEqual(42,
                parameter.Size);
        }

        /// <summary>
        /// Retains parameter direction and nullability.
        /// </summary>
        [TestMethod]
        public void ParameterPreservesBindingMetadata()
        {
            // Setup
            var parameter = new CatalogParameter();

            // Act
            parameter.Direction = ParameterDirection.InputOutput;
            parameter.IsNullable = true;
            parameter.ParameterName = "@Id";

            // Assert
            Assert.AreEqual(ParameterDirection.InputOutput,
                parameter.Direction);
            Assert.IsTrue(parameter.IsNullable);
            Assert.AreEqual("@Id",
                parameter.ParameterName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Finds parameter names without case sensitivity.
        /// </summary>
        [TestMethod]
        public void ParametersFindInstancesAndNames()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            var actual = parameters.IndexOf("@ID");

            // Assert
            Assert.AreEqual(0,
                actual);
            Assert.IsTrue(parameters.Contains(first));
            Assert.IsTrue(parameters.Contains("@id"));
        }

        /// <summary>
        /// Supports both parameter indexers.
        /// </summary>
        [TestMethod]
        public void ParametersReplaceByNameAndPosition()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters[0] = new CatalogParameter { ParameterName = "@Id", Value = 2 };
            parameters["@Id"] = first;

            // Assert
            Assert.AreSame(first,
                parameters[0]);
            Assert.AreSame(first,
                parameters["@Id"]);
        }

        /// <summary>
        /// Adds, copies and enumerates parameter ranges.
        /// </summary>
        [TestMethod]
        public void ParametersCopyAndEnumerateAddedRange()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.AddRange(new[] { new CatalogParameter() });
            var copied = new object[2];
            parameters.CopyTo(copied,
                0);

            // Assert
            Assert.AreEqual(2,
                parameters.Cast<object>().Count());
            Assert.AreSame(first,
                copied[0]);
            Assert.IsNotNull(parameters.SyncRoot);
        }

        /// <summary>
        /// Inserts a parameter before existing entries.
        /// </summary>
        [TestMethod]
        public void ParametersInsertAtRequestedPosition()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.Insert(0,
                new CatalogParameter());

            // Assert
            Assert.AreEqual(1,
                parameters.IndexOf(first));
        }

        /// <summary>
        /// Removes a parameter instance.
        /// </summary>
        [TestMethod]
        public void ParametersRemoveByInstance()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.Remove(first);

            // Assert
            Assert.AreEqual(0,
                parameters.Count);
        }

        /// <summary>
        /// Removes a parameter by name.
        /// </summary>
        [TestMethod]
        public void ParametersRemoveByName()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.RemoveAt("@Id");

            // Assert
            Assert.AreEqual(0,
                parameters.Count);
        }

        /// <summary>
        /// Removes a parameter by position.
        /// </summary>
        [TestMethod]
        public void ParametersRemoveByPosition()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.RemoveAt(0);

            // Assert
            Assert.AreEqual(0,
                parameters.Count);
        }

        /// <summary>
        /// Clears every parameter.
        /// </summary>
        [TestMethod]
        public void ParametersClearAllEntries()
        {
            // Setup
            var parameters = new CatalogParameterCollection();
            var first = new CatalogParameter { ParameterName = "@Id", Value = 1 };
            parameters.Add(first);

            // Act
            parameters.Clear();

            // Assert
            Assert.AreEqual(0,
                parameters.Count);
        }

        #endregion
    }
}
