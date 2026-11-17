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
    /// <summary>
    /// Contains the save definition test base behavior.
    /// </summary>
    public abstract class SaveDefinitionTestBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        protected TableSchema Customer { get; private set; }

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        protected string DirectoryPath { get; private set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        protected string Filename => Path.Combine(DirectoryPath,
            "schema.txt");

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        protected TableSchema Invoice { get; private set; }

        /// <summary>
        /// Gets or sets the reader.
        /// </summary>
        protected Mock<ISchemaReader> Reader { get; private set; }

        #endregion

        #region Initialization and Cleanup

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DirectoryPath = Path.Combine(Path.GetTempPath(),
                "repodb-export-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
            Invoice = new TableSchema(new TableInfo("Invoice",
                    "sales",
                    "Source",
                    "SqlServer"));
            Customer = new TableSchema(new TableInfo("Customer",
                    "sales",
                    "Source",
                    "SqlServer"));
            Invoice.Columns.Add(new ColumnInfo { Field = new DbField("Amount",
                    false,
                    false,
                    false,
                    typeof(decimal),
                    null,
                    18,
                    4,
                    "decimal",
                    true,
                    "SqlServer"), Ordinal = 2, DefaultExpression = "((0))", Comment = "A: 'quote'\n雪 & <xml>" });
            Invoice.Columns.Add(new ColumnInfo { Field = new DbField("Name",
                    false,
                    false,
                    true,
                    typeof(string),
                    100,
                    null,
                    null,
                    "nvarchar",
                    false,
                    "SqlServer"), Ordinal = 1, Collation = "Latin1_General_100_CI_AS", IsUnicode = true, CharacterSet = "UTF-16", LengthUnit = "utf16CodeUnits" });
            Invoice.PrimaryKey = new PrimaryKeyInfo("PK_Invoice")
            {
                Columns = new[]
                {
                    "Name",
                    "Amount"
                },
                IsClustered = false
            };
            Invoice.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Invoice") { Columns = new[] { "Name", "Amount" } });
            Invoice.Indexes.Add(new IndexInfo("IX_Invoice") { Columns = new[] { "Name" }, IncludedColumns = new[] { "Amount" }, DescendingColumns = new[] { "Name" }, Filter = "[Amount] > 0", IsUnique = true });
            Invoice.CheckConstraints.Add(new CheckConstraintInfo("CK_Invoice") { Expression = "[Amount] >= 0" });
            Invoice.ForeignKeys.Add(new ForeignKeyInfo("FK_Invoice_Customer") { Columns = new[] { "Name", "Amount" }, ReferencedTable = Customer.Table, ReferencedColumns = new[] { "Name", "Amount" }, DeleteRule = ForeignKeyRule.Cascade });
            Reader = new Mock<ISchemaReader>(MockBehavior.Strict);
            Reader.Setup(r => r.TableExists(It.IsAny<System.Data.IDbConnection>(),
                    "sales.Invoice",
                    It.IsAny<System.Data.IDbTransaction>())).Returns(true);
            Reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    "sales.Invoice",
                    It.IsAny<System.Data.IDbTransaction>())).Returns(Invoice);
            Reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    "sales.Customer",
                    It.IsAny<System.Data.IDbTransaction>())).Returns(Customer);
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(new[] { "sales.Invoice", "sales.Customer" });
            SchemaReaderMapper.Add<CustomDbConnection>(Reader.Object,
                true);
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Directory.Delete(DirectoryPath,
                true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs capture exception.
        /// </summary>
        /// <param name="action">The action.</param>
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
