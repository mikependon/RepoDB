#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaTraceKeysTest
    {
        #region Methods

        [TestMethod]
        public void TestSchemaTraceKeysResolveSchemaName()
        {
            // Act
            var actual = SchemaTraceKeys.ResolveSchemaName;
            var expected = "ResolveSchemaName";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysTableExists()
        {
            // Act
            var actual = SchemaTraceKeys.TableExists;
            var expected = "TableExists";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetColumns()
        {
            // Act
            var actual = SchemaTraceKeys.GetColumns;
            var expected = "GetColumns";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetPrimaryKey()
        {
            // Act
            var actual = SchemaTraceKeys.GetPrimaryKey;
            var expected = "GetPrimaryKey";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetIndexes()
        {
            // Act
            var actual = SchemaTraceKeys.GetIndexes;
            var expected = "GetIndexes";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetForeignKeys()
        {
            // Act
            var actual = SchemaTraceKeys.GetForeignKeys;
            var expected = "GetForeignKeys";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetUniqueConstraints()
        {
            // Act
            var actual = SchemaTraceKeys.GetUniqueConstraints;
            var expected = "GetUniqueConstraints";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetCheckConstraints()
        {
            // Act
            var actual = SchemaTraceKeys.GetCheckConstraints;
            var expected = "GetCheckConstraints";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysGetTables()
        {
            // Act
            var actual = SchemaTraceKeys.GetTables;
            var expected = "GetTables";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysCopySchemaTo()
        {
            // Act
            var actual = SchemaTraceKeys.CopySchemaTo;
            var expected = "CopySchemaTo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaTraceKeysCopySchemasTo()
        {
            // Act
            var actual = SchemaTraceKeys.CopySchemasTo;
            var expected = "CopySchemasTo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        #endregion
    }
}
