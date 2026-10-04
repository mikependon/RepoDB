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
    public class CopySchemasResultTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemasResultTablesPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemasResult();
            var actual = result.Tables;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopySchemasResultTablesProperty()
        {
            // Setup
            var table = new CopySchemaResult { TableName = "Person" };

            // Act
            var result = new CopySchemasResult();
            result.Tables.Add(table);
            var actual = result.Tables;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(table, actual[0]);
        }

        [TestMethod]
        public void TestCopySchemasResultTablesAreNotShared()
        {
            // Act
            var first = new CopySchemasResult();
            var second = new CopySchemasResult();
            first.Tables.Add(new CopySchemaResult());

            // Assert
            Assert.AreEqual(0, second.Tables.Count);
        }

        [TestMethod]
        public void TestCopySchemasResultTableCountPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemasResult();

            // Assert
            Assert.AreEqual(0, result.TableCount);
        }

        [TestMethod]
        public void TestCopySchemasResultTableCountProperty()
        {
            // Act
            var result = new CopySchemasResult();
            result.Tables.Add(new CopySchemaResult());
            result.Tables.Add(new CopySchemaResult());

            // Assert
            Assert.AreEqual(2, result.TableCount);
        }

        [TestMethod]
        public void TestCopySchemasResultActionPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemasResult();

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.SkipOnExists, result.Action);
        }

        [TestMethod]
        public void TestCopySchemasResultActionProperty()
        {
            // Act
            var result = new CopySchemasResult
            {
                Action = CopySchemaExistsBehavior.DropOnExists
            };

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, result.Action);
        }

        [TestMethod]
        public void TestCopySchemasResultStringProperties()
        {
            // Act
            var result = new CopySchemasResult
            {
                Script = "CREATE TABLE [Person] ([Id] int);",
                SourceDatabase = "Source",
                SourceDatabaseType = "SqlConnection",
                SourceServer = "SourceServer",
                DestinationDatabase = "Destination",
                DestinationDatabaseType = "NpgsqlConnection",
                DestinationServer = "DestinationServer"
            };

            // Assert
            Assert.AreEqual("CREATE TABLE [Person] ([Id] int);", result.Script, StringComparer.Ordinal);
            Assert.AreEqual("Source", result.SourceDatabase, StringComparer.Ordinal);
            Assert.AreEqual("SqlConnection", result.SourceDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual("SourceServer", result.SourceServer, StringComparer.Ordinal);
            Assert.AreEqual("Destination", result.DestinationDatabase, StringComparer.Ordinal);
            Assert.AreEqual("NpgsqlConnection", result.DestinationDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual("DestinationServer", result.DestinationServer, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopySchemasResultStringPropertiesDefaultValues()
        {
            // Act
            var result = new CopySchemasResult();

            // Assert
            Assert.IsNull(result.Script);
            Assert.IsNull(result.SourceDatabase);
            Assert.IsNull(result.DestinationDatabase);
        }

        [TestMethod]
        public void TestCopySchemasResultTimeProperties()
        {
            // Act
            var result = new CopySchemasResult
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc)
            };

            // Assert
            Assert.AreEqual(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc), result.StartTime);
            Assert.AreEqual(new DateTime(2026, 1, 1, 10, 0, 5, DateTimeKind.Utc), result.EndTime);
            Assert.AreEqual(TimeSpan.FromSeconds(5), result.Duration);
        }

        [TestMethod]
        public void TestCopySchemasResultDurationPropertyDefaultValue()
        {
            // Act
            var result = new CopySchemasResult();

            // Assert
            Assert.AreEqual(TimeSpan.Zero, result.Duration);
        }

        #endregion
    }
}
