#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaColumnMetadataTest
    {
        #region Methods

        [TestMethod]
        public void TestSchemaColumnMetadataPreservesCharacterSet()
        {
            // Setup
            var data = new DataTable();
            data.Columns.Add("CharacterSet", typeof(string));
            data.Columns.Add("TemporalPrecision", typeof(int));
            data.Columns.Add("Collation", typeof(string));
            data.Rows.Add("utf8mb4", 6, "utf8mb4_bin");
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "MySql")
            };

            // Act
            var actual = SchemaColumnMetadata.Apply(column, record);

            // Assert
            Assert.AreEqual("utf8mb4", actual.CharacterSet);
        }

        [TestMethod]
        public void TestSchemaColumnMetadataPreservesUnicodeFlag()
        {
            // Setup
            var data = new DataTable();
            data.Columns.Add("CharacterSet", typeof(string));
            data.Columns.Add("TemporalPrecision", typeof(int));
            data.Columns.Add("Collation", typeof(string));
            data.Rows.Add("utf8mb4", 6, "utf8mb4_bin");
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "MySql")
            };

            // Act
            var actual = SchemaColumnMetadata.Apply(column, record);

            // Assert
            Assert.IsTrue(actual.IsUnicode.Value);
        }

        [TestMethod]
        public void TestSchemaColumnMetadataPreservesLengthUnit()
        {
            // Setup
            var data = new DataTable();
            data.Columns.Add("CharacterSet", typeof(string));
            data.Columns.Add("TemporalPrecision", typeof(int));
            data.Columns.Add("Collation", typeof(string));
            data.Rows.Add("utf8mb4", 6, "utf8mb4_bin");
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "MySql")
            };

            // Act
            var actual = SchemaColumnMetadata.Apply(column, record);

            // Assert
            Assert.AreEqual("characters", actual.LengthUnit);
        }

        [TestMethod]
        public void TestSchemaColumnMetadataPreservesCollation()
        {
            // Setup
            var data = new DataTable();
            data.Columns.Add("CharacterSet", typeof(string));
            data.Columns.Add("TemporalPrecision", typeof(int));
            data.Columns.Add("Collation", typeof(string));
            data.Rows.Add("utf8mb4", 6, "utf8mb4_bin");
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "MySql")
            };

            // Act
            var actual = SchemaColumnMetadata.Apply(column, record);

            // Assert
            Assert.AreEqual("utf8mb4_bin", actual.Collation);
        }

        [TestMethod]
        public void TestSchemaColumnMetadataPreservesTemporalPrecision()
        {
            // Setup
            var data = new DataTable();
            data.Columns.Add("CharacterSet", typeof(string));
            data.Columns.Add("TemporalPrecision", typeof(int));
            data.Columns.Add("Collation", typeof(string));
            data.Rows.Add("utf8mb4", 6, "utf8mb4_bin");
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "MySql")
            };

            // Act
            var actual = SchemaColumnMetadata.Apply(column, record);

            // Assert
            Assert.AreEqual(6, actual.TemporalPrecision);
        }

        #endregion
    }
}
