#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    public abstract class SaveDefinitionTestBase
    {
        #region Properties

        protected TableSchema Customer { get; private set; }

        protected string DirectoryPath { get; private set; }

        protected string Filename => Path.Combine(DirectoryPath, "schema.txt");

        protected TableSchema Invoice { get; private set; }

        protected Mock<ISchemaReader> Reader { get; private set; }

        #endregion

        #region Initialization and Cleanup

        [TestInitialize]
        public void Initialize()
        {
            DirectoryPath = Path.Combine(Path.GetTempPath(), "repodb-export-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
            Invoice = new TableSchema(new TableInfo("Invoice", "sales", "Source", "SqlServer"));
            Customer = new TableSchema(new TableInfo("Customer", "sales", "Source", "SqlServer"));
            Invoice.Columns.Add(new ColumnInfo
            {
                Field = new DbField("Amount", false, false, false, typeof(decimal), null, 18, 4, "decimal", true, "SqlServer"),
                Ordinal = 2, DefaultExpression = "((0))", Comment = "A: 'quote'\n雪 & <xml>"
            });
            Invoice.Columns.Add(new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "nvarchar", false, "SqlServer"),
                Ordinal = 1, Collation = "Latin1_General_100_CI_AS"
            });
            Invoice.PrimaryKey = new PrimaryKeyInfo("PK_Invoice") { Columns = new[] { "Name", "Amount" }, IsClustered = false };
            Invoice.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Invoice") { Columns = new[] { "Name", "Amount" } });
            Invoice.Indexes.Add(new IndexInfo("IX_Invoice")
            {
                Columns = new[] { "Name" }, IncludedColumns = new[] { "Amount" }, DescendingColumns = new[] { "Name" },
                Filter = "[Amount] > 0", IsUnique = true
            });
            Invoice.CheckConstraints.Add(new CheckConstraintInfo("CK_Invoice") { Expression = "[Amount] >= 0" });
            Invoice.ForeignKeys.Add(new ForeignKeyInfo("FK_Invoice_Customer")
            {
                Columns = new[] { "Name", "Amount" }, ReferencedTable = Customer.Table,
                ReferencedColumns = new[] { "Name", "Amount" }, DeleteRule = ForeignKeyRule.Cascade
            });
            Reader = new Mock<ISchemaReader>(MockBehavior.Strict);
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(true);
            Reader.Setup(r => r.GetTableSchema("sales.Invoice")).Returns(Invoice);
            Reader.Setup(r => r.GetTableSchema("sales.Customer")).Returns(Customer);
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>()))
                .Returns(new[] { "sales.Invoice", "sales.Customer" });
            SchemaReaderMapper.Add<CustomDbConnection>(Reader.Object, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Directory.Delete(DirectoryPath, true);
        }

        #endregion

        #region Helpers

        protected static Exception CaptureException(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        #endregion
    }
}
