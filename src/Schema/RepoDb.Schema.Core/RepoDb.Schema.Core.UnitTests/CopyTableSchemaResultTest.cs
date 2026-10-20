#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class CopyTableSchemaResultTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaResultAddedColumnsPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.AddedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultAddedIndexesPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.AddedIndexes;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultWarningsPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Warnings;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultActionPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Action;

            // Assert
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultActionProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                Action = CopyTableSchemaExistsBehavior.Drop
            };
            var actual = result.Action;
            var expected = CopyTableSchemaExistsBehavior.Drop;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultOutcomePropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Outcome;

            // Assert
            Assert.AreEqual(CopyTableSchemaOutcome.Created, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultOutcomeProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                Outcome = CopyTableSchemaOutcome.Aligned
            };
            var actual = result.Outcome;
            var expected = CopyTableSchemaOutcome.Aligned;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultTableNameProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                TableName = "Person"
            };
            var actual = result.TableName;
            var expected = "Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultScriptProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                Script = "CREATE TABLE [Person] ([Id] int);"
            };
            var actual = result.Script;
            var expected = "CREATE TABLE [Person] ([Id] int);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultSourceDatabaseProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                SourceDatabase = "Source"
            };
            var actual = result.SourceDatabase;
            var expected = "Source";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultSourceDatabaseTypeProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                SourceDatabaseType = "SqlConnection"
            };
            var actual = result.SourceDatabaseType;
            var expected = "SqlConnection";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultSourceSchemaProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                SourceSchema = "dbo"
            };
            var actual = result.SourceSchema;
            var expected = "dbo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultSourceServerProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                SourceServer = "SourceServer"
            };
            var actual = result.SourceServer;
            var expected = "SourceServer";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDestinationDatabaseProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                DestinationDatabase = "Destination"
            };
            var actual = result.DestinationDatabase;
            var expected = "Destination";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDestinationDatabaseTypeProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                DestinationDatabaseType = "NpgsqlConnection"
            };
            var actual = result.DestinationDatabaseType;
            var expected = "NpgsqlConnection";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDestinationSchemaProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                DestinationSchema = "public"
            };
            var actual = result.DestinationSchema;
            var expected = "public";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDestinationServerProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                DestinationServer = "DestinationServer"
            };
            var actual = result.DestinationServer;
            var expected = "DestinationServer";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultColumnCountProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                ColumnCount = 4
            };
            var actual = result.ColumnCount;
            var expected = 4;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultIndexCountProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                IndexCount = 2
            };
            var actual = result.IndexCount;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultForeignKeyCountProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                ForeignKeyCount = 1
            };
            var actual = result.ForeignKeyCount;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultUniqueConstraintCountProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                UniqueConstraintCount = 3
            };
            var actual = result.UniqueConstraintCount;
            var expected = 3;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultCheckConstraintCountProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                CheckConstraintCount = 5
            };
            var actual = result.CheckConstraintCount;
            var expected = 5;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultTableExistedPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.TableExisted;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultTableExistedProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                TableExisted = true
            };
            var actual = result.TableExisted;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDurationProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc)
            };
            var actual = result.Duration;
            var expected = TimeSpan.FromSeconds(5);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultDurationPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Duration;

            // Assert
            Assert.AreEqual(TimeSpan.Zero, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultStartTimeProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            };
            var actual = result.StartTime;
            var expected = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultEndTimeProperty()
        {
            // Act
            var result = new CopyTableSchemaResult
            {
                EndTime = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc)
            };
            var actual = result.EndTime;
            var expected = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultErrorsPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Errors;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultErrorsProperty()
        {
            // Setup
            var error = new CopyTableSchemaError { Statement = "CREATE TABLE [Person] ([Id] int);" };

            // Act
            var result = new CopyTableSchemaResult();
            result.Errors.Add(error);
            var actual = result.Errors;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(error, actual[0]);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultErrorsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResult();
            var second = new CopyTableSchemaResult();
            first.Errors.Add(new CopyTableSchemaError());

            // Assert
            Assert.AreEqual(0, second.Errors.Count);
        }

        #endregion
    }
}
