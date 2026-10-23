#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class CopyTableSchemaResultTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaResultItemsPropertyDefaultValue()
        {
            // Act
            var result = new CopyTableSchemaResult();
            var actual = result.Items;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemsProperty()
        {
            // Setup
            var item = new CopyTableSchemaResultItem();

            // Act
            var result = new CopyTableSchemaResult();
            result.Items.Add(item);
            var actual = result.Items;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(item, actual[0]);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultItemsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResult();
            var second = new CopyTableSchemaResult();
            first.Items.Add(new CopyTableSchemaResultItem());

            // Assert
            Assert.AreEqual(0, second.Items.Count);
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
        public void TestCopyTableSchemaResultWarningsProperty()
        {
            // Act
            var result = new CopyTableSchemaResult();
            result.Warnings.Add("Skipped the check constraint 'CK_Person'.");
            var actual = result.Warnings;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Skipped the check constraint 'CK_Person'.", actual[0], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaResultWarningsAreNotShared()
        {
            // Act
            var first = new CopyTableSchemaResult();
            var second = new CopyTableSchemaResult();
            first.Warnings.Add("Warning");

            // Assert
            Assert.AreEqual(0, second.Warnings.Count);
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
            var error = new ExecutionError { Statement = "CREATE TABLE [Person] ([Id] int);" };

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
            first.Errors.Add(new ExecutionError());

            // Assert
            Assert.AreEqual(0, second.Errors.Count);
        }

        #endregion
    }
}
