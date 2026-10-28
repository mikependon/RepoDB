#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class LoadDefinitionTransactionTest : SaveDefinitionTestBase
    {
        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task CallerCanRollBackAllCreatedTables(string format, bool asynchronous)
        {
            if (format == "json") Connection.SaveDefinitionAsJson("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren);
            else if (format == "xml") Connection.SaveDefinitionAsXml("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren);
            else Connection.SaveDefinitionAsYaml("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren);
            using var target = new SqliteConnection("Data Source=:memory:");
            await target.OpenAsync();
            var callbacks = new List<CopyTableSchemaResultItem>();
            using (var transaction = target.BeginTransaction())
            {
                LoadDefinitionResult result;
                if (format == "json") result = asynchronous
                    ? await target.LoadDefinitionFromJsonAsync(Filename, createdCallback: callbacks.Add, transaction: transaction)
                    : target.LoadDefinitionFromJson(Filename, createdCallback: callbacks.Add, transaction: transaction);
                else if (format == "xml") result = asynchronous
                    ? await target.LoadDefinitionFromXmlAsync(Filename, createdCallback: callbacks.Add, transaction: transaction)
                    : target.LoadDefinitionFromXml(Filename, createdCallback: callbacks.Add, transaction: transaction);
                else result = asynchronous
                    ? await target.LoadDefinitionFromYamlAsync(Filename, createdCallback: callbacks.Add, transaction: transaction)
                    : target.LoadDefinitionFromYaml(Filename, createdCallback: callbacks.Add, transaction: transaction);
                Assert.AreEqual(3, result.CopyResult.Items.Count);
                Assert.AreEqual(3, callbacks.Count);
                using var command = target.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name IN ('Invoice', 'InvoiceLine', 'Customer');";
                Assert.AreEqual(3L, command.ExecuteScalar());
                transaction.Rollback();
            }
            using var check = target.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name IN ('Invoice', 'InvoiceLine', 'Customer');";
            Assert.AreEqual(0L, check.ExecuteScalar());
        }
    }
}
