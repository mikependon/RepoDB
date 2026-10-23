#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Schema.Models;
using DbConnectionBase = System.Data.Common.DbConnection;

namespace RepoDb.Schema.Sqlite.Microsoft.UnitTests
{
    [TestClass]
    public class SchemaReaderProviderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MappedDbConnection>(new MappedDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Remove<MappedDbConnection>();
        }

        [TestMethod]
        public void TestSqliteSchemaReaderNewTableGetsTheDatabaseOfTheConnectionAndTheProviderOfTheDbSetting()
        {
            // Setup
            var reader = new SqliteSchemaReader(new MappedDbConnection());

            // Act
            var actual = (TableInfo)typeof(SqliteSchemaReader).GetMethod("NewTable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(reader, new object[] { "Person", "dbo" });

            // Assert
            Assert.AreEqual("Person", actual.Name);
            Assert.AreEqual("dbo", actual.Schema);
            Assert.AreEqual("FakeDatabase", actual.Database);
            Assert.AreEqual("FAKE", actual.Provider);
        }

        [TestMethod]
        public void TestSqliteSchemaReaderProviderIsTheProviderOfTheDbSettingOfTheConnection()
        {
            // Setup
            var reader = new SqliteSchemaReader(new MappedDbConnection());

            // Act
            var actual = (string)typeof(SqliteSchemaReader).GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);

            // Assert
            Assert.AreEqual("FAKE", actual);
        }

        [TestMethod]
        public void TestSqliteSchemaReaderProviderIsNullIfTheConnectionHasNoDbSetting()
        {
            // Setup
            var reader = new SqliteSchemaReader(new UnmappedDbConnection());

            // Act
            var provider = (string)typeof(SqliteSchemaReader).GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);
            var table = (TableInfo)typeof(SqliteSchemaReader).GetMethod("NewTable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(reader, new object[] { "Person", "dbo" });

            // Assert
            Assert.IsNull(provider);
            Assert.IsNull(table.Provider);
            Assert.AreEqual("FakeDatabase", table.Database);
        }

        private sealed class MappedDbSetting : BaseDbSetting
        {
            public MappedDbSetting()
            {
                Provider = "FAKE";
            }
        }

        private class FakeDbConnection : DbConnectionBase
        {
            public override string ConnectionString { get; set; }

            public override string Database => "FakeDatabase";

            public override string DataSource => "FakeDataSource";

            public override string ServerVersion => "1.0";

            public override ConnectionState State => ConnectionState.Open;

            public override void ChangeDatabase(string databaseName)
            {
            }

            public override void Close()
            {
            }

            public override void Open()
            {
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => null;

            protected override DbCommand CreateDbCommand() => null;
        }

        private sealed class MappedDbConnection : FakeDbConnection
        {
        }

        private sealed class UnmappedDbConnection : FakeDbConnection
        {
        }
    }
}
