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
    public class CopySchemaResultTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemaResultAddedColumnsPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.AddedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopySchemaResultAddedIndexesPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.AddedIndexes;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopySchemaResultWarningsPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.Warnings;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopySchemaResultActionPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.Action;

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.SkipOnExists, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultActionProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                Action = CopySchemaExistsBehavior.DropOnExists
            };
            var actual = result.Action;
            var expected = CopySchemaExistsBehavior.DropOnExists;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultOutcomePropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.Outcome;

            // Assert
            Assert.AreEqual(CopySchemaOutcome.Created, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultOutcomeProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                Outcome = CopySchemaOutcome.Aligned
            };
            var actual = result.Outcome;
            var expected = CopySchemaOutcome.Aligned;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultTableNameProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                TableName = "Person"
            };
            var actual = result.TableName;
            var expected = "Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultScriptProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                Script = "CREATE TABLE [Person] ([Id] int);"
            };
            var actual = result.Script;
            var expected = "CREATE TABLE [Person] ([Id] int);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultSourceDatabaseProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                SourceDatabase = "Source"
            };
            var actual = result.SourceDatabase;
            var expected = "Source";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultSourceDatabaseTypeProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                SourceDatabaseType = "SqlConnection"
            };
            var actual = result.SourceDatabaseType;
            var expected = "SqlConnection";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultSourceSchemaProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                SourceSchema = "dbo"
            };
            var actual = result.SourceSchema;
            var expected = "dbo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultSourceServerProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                SourceServer = "SourceServer"
            };
            var actual = result.SourceServer;
            var expected = "SourceServer";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultDestinationDatabaseProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                DestinationDatabase = "Destination"
            };
            var actual = result.DestinationDatabase;
            var expected = "Destination";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultDestinationDatabaseTypeProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                DestinationDatabaseType = "NpgsqlConnection"
            };
            var actual = result.DestinationDatabaseType;
            var expected = "NpgsqlConnection";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultDestinationSchemaProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                DestinationSchema = "public"
            };
            var actual = result.DestinationSchema;
            var expected = "public";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultDestinationServerProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                DestinationServer = "DestinationServer"
            };
            var actual = result.DestinationServer;
            var expected = "DestinationServer";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemaResultColumnCountProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                ColumnCount = 4
            };
            var actual = result.ColumnCount;
            var expected = 4;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultIndexCountProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                IndexCount = 2
            };
            var actual = result.IndexCount;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultForeignKeyCountProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                ForeignKeyCount = 1
            };
            var actual = result.ForeignKeyCount;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultUniqueConstraintCountProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                UniqueConstraintCount = 3
            };
            var actual = result.UniqueConstraintCount;
            var expected = 3;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultCheckConstraintCountProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                CheckConstraintCount = 5
            };
            var actual = result.CheckConstraintCount;
            var expected = 5;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultTableExistedPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.TableExisted;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCopySchemaResultTableExistedProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                TableExisted = true
            };
            var actual = result.TableExisted;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultDurationProperty()
        {
            // Act
            var result = new CopySchemaResult
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
        public void TestCopySchemaResultDurationPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.Duration;

            // Assert
            Assert.AreEqual(TimeSpan.Zero, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultStartTimeProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            };
            var actual = result.StartTime;
            var expected = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultEndTimeProperty()
        {
            // Act
            var result = new CopySchemaResult
            {
                EndTime = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc)
            };
            var actual = result.EndTime;
            var expected = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaResultErrorsPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemaResult();
            var actual = result.Errors;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopySchemaResultErrorsProperty()
        {
            // Setup
            var error = new CopySchemaError { Statement = "CREATE TABLE [Person] ([Id] int);" };

            // Act
            var result = new CopySchemaResult();
            result.Errors.Add(error);
            var actual = result.Errors;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(error, actual[0]);
        }

        [TestMethod]
        public void TestCopySchemaResultErrorsAreNotShared()
        {
            // Act
            var first = new CopySchemaResult();
            var second = new CopySchemaResult();
            first.Errors.Add(new CopySchemaError());

            // Assert
            Assert.AreEqual(0, second.Errors.Count);
        }

        #endregion
    }
}
