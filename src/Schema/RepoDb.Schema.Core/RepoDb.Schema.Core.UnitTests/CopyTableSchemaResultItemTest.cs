#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class CopyTableSchemaResultItemTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaResultItemSourcePropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Source;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNull(actual.Table);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemTargetPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Target;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNull(actual.Table);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemSourceAndTargetAreNotTheSame()
        {
            // Act
            var item = new CopyTableSchemaResultItem();

            // Assert
            Assert.AreNotSame(item.Source, item.Target);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemSourceAndTargetAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResultItem();
            var second = new CopyTableSchemaResultItem();
            first.Source.Server = "SourceServer";
            first.Target.Server = "TargetServer";

            // Assert
            Assert.IsNull(second.Source.Server);
            Assert.IsNull(second.Target.Server);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemSourceAndTargetProperties()
        {
            // Setup
            var source = new Endpoint
            {
                Table = new TableSchema("Person", "dbo"),
                Database = "Source",
                DatabaseType = "SqlConnection",
                Server = "SourceServer"
            };
            var target = new Endpoint
            {
                Table = new TableSchema("Person", "public"),
                Database = "Target",
                DatabaseType = "NpgsqlConnection",
                Server = "TargetServer"
            };

            // Act
            var item = new CopyTableSchemaResultItem
            {
                Source = source,
                Target = target
            };

            // Assert
            Assert.AreSame(source, item.Source);
            Assert.AreSame(target, item.Target);
            Assert.AreEqual("dbo", item.Source.Table.Table.Schema, StringComparer.Ordinal);
            Assert.AreEqual("public", item.Target.Table.Table.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemAddedColumnsPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.AddedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemAddedIndexesPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.AddedIndexes;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemAddedForeignKeysPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.AddedForeignKeys;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemAddedListsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResultItem();
            var second = new CopyTableSchemaResultItem();
            first.AddedColumns.Add("Name");
            first.AddedIndexes.Add("IX_Person");
            first.AddedForeignKeys.Add("FK_Person");

            // Assert
            Assert.AreEqual(0, second.AddedColumns.Count);
            Assert.AreEqual(0, second.AddedIndexes.Count);
            Assert.AreEqual(0, second.AddedForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemWarningsPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Warnings;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemWarningsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResultItem();
            var second = new CopyTableSchemaResultItem();
            first.Warnings.Add("Warning");

            // Assert
            Assert.AreEqual(0, second.Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemErrorsPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Errors;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemErrorsProperty()
        {
            // Setup
            var error = new ExecutionError { Statement = "CREATE TABLE [Person] ([Id] int);" };

            // Act
            var item = new CopyTableSchemaResultItem();
            item.Errors.Add(error);
            var actual = item.Errors;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(error, actual[0]);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemErrorsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResultItem();
            var second = new CopyTableSchemaResultItem();
            first.Errors.Add(new ExecutionError());

            // Assert
            Assert.AreEqual(0, second.Errors.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemActionPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Action;

            // Assert
            Assert.AreEqual(TableExistsBehavior.Skip, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemActionProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                Action = TableExistsBehavior.Drop
            };
            var actual = item.Action;
            var expected = TableExistsBehavior.Drop;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemOutcomePropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.Outcome;

            // Assert
            Assert.AreEqual(TableOutcome.Created, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemOutcomeProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                Outcome = TableOutcome.Aligned
            };
            var actual = item.Outcome;
            var expected = TableOutcome.Aligned;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemScriptProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                Script = "CREATE TABLE [Person] ([Id] int);"
            };
            var actual = item.Script;
            var expected = "CREATE TABLE [Person] ([Id] int);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemColumnCountProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                ColumnCount = 4
            };
            var actual = item.ColumnCount;
            var expected = 4;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemIndexCountProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                IndexCount = 2
            };
            var actual = item.IndexCount;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemForeignKeyCountProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                ForeignKeyCount = 1
            };
            var actual = item.ForeignKeyCount;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemUniqueConstraintCountProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                UniqueConstraintCount = 3
            };
            var actual = item.UniqueConstraintCount;
            var expected = 3;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemCheckConstraintCountProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                CheckConstraintCount = 5
            };
            var actual = item.CheckConstraintCount;
            var expected = 5;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemTableExistedPropertyDefaultValue()
        {
            // Act
            var item = new CopyTableSchemaResultItem();
            var actual = item.TableExisted;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemTableExistedProperty()
        {
            // Act
            var item = new CopyTableSchemaResultItem
            {
                TableExisted = true
            };
            var actual = item.TableExisted;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
