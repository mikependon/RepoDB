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

namespace RepoDb.Schema.Sqlite.Turso.UnitTests
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
        public void TestTursoSchemaReaderNewTableGetsTheDatabaseOfTheConnectionAndTheProviderOfTheDbSetting()
        {
            // Setup
            var reader = new TursoSchemaReader(new MappedDbConnection());

            // Act
            var actual = (TableInfo)typeof(TursoSchemaReader).GetMethod("NewTable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(reader, new object[] { "Person", "dbo" });

            // Assert
            Assert.AreEqual("Person", actual.Name);
            Assert.AreEqual("dbo", actual.Schema);
            Assert.AreEqual("FakeDatabase", actual.Database);
            Assert.AreEqual("FAKE", actual.Provider);
        }

        [TestMethod]
        public void TestTursoSchemaReaderProviderIsTheProviderOfTheDbSettingOfTheConnection()
        {
            // Setup
            var reader = new TursoSchemaReader(new MappedDbConnection());

            // Act
            var actual = (string)typeof(TursoSchemaReader).GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);

            // Assert
            Assert.AreEqual("FAKE", actual);
        }

        [TestMethod]
        public void TestTursoSchemaReaderProviderIsNullIfTheConnectionHasNoDbSetting()
        {
            // Setup
            var reader = new TursoSchemaReader(new UnmappedDbConnection());

            // Act
            var provider = (string)typeof(TursoSchemaReader).GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);
            var table = (TableInfo)typeof(TursoSchemaReader).GetMethod("NewTable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(reader, new object[] { "Person", "dbo" });

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
