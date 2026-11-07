using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForNull()
        {
            // Setup
            Type type = null;
            string expected = "unknown";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForUri()
        {
            // Setup
            Type type = typeof(Uri);
            string expected = "unknown";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForChar()
        {
            // Setup
            Type type = typeof(char);
            string expected = "string";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForByteArray()
        {
            // Setup
            Type type = typeof(byte[]);
            string expected = "binary";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForGuid()
        {
            // Setup
            Type type = typeof(Guid);
            string expected = "uuid";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForDateTimeOffset()
        {
            // Setup
            Type type = typeof(DateTimeOffset);
            string expected = "datetimeOffset";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForTimeSpan()
        {
            // Setup
            Type type = typeof(TimeSpan);
            string expected = "time";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForBoolean()
        {
            // Setup
            Type type = typeof(bool);
            string expected = "boolean";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForByte()
        {
            // Setup
            Type type = typeof(byte);
            string expected = "uint8";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForSByte()
        {
            // Setup
            Type type = typeof(sbyte);
            string expected = "int8";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForInt16()
        {
            // Setup
            Type type = typeof(short);
            string expected = "int16";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForUInt16()
        {
            // Setup
            Type type = typeof(ushort);
            string expected = "uint16";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForInt32()
        {
            // Setup
            Type type = typeof(int);
            string expected = "int32";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForUInt32()
        {
            // Setup
            Type type = typeof(uint);
            string expected = "uint32";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForInt64()
        {
            // Setup
            Type type = typeof(long);
            string expected = "int64";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForUInt64()
        {
            // Setup
            Type type = typeof(ulong);
            string expected = "uint64";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForDecimal()
        {
            // Setup
            Type type = typeof(decimal);
            string expected = "decimal";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForSingle()
        {
            // Setup
            Type type = typeof(float);
            string expected = "float32";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForDouble()
        {
            // Setup
            Type type = typeof(double);
            string expected = "float64";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForDateTime()
        {
            // Setup
            Type type = typeof(DateTime);
            string expected = "datetime";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Describes portable CLR types without reflection-based serialization.
        /// </summary>
        [TestMethod]
        public void CreatePreservesPortableLogicalTypeForNullableInt32()
        {
            // Setup
            Type type = typeof(int?);
            string expected = "int32";
            var column = CreateColumn(type,
                "custom",
                "Other");

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.AreEqual(expected,
                actual["logical"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Keeps explicit metadata ahead of provider defaults.
        /// </summary>
        [TestMethod]
        public void CreatePreservesExplicitEncodingMetadata()
        {
            // Setup
            var column = CreateColumn(typeof(string),
                "nvarchar",
                "SqlServer");
            column.IsUnicode = false;
            column.CharacterSet = "Custom";
            column.LengthUnit = "characters";

            // Act
            var actual = ColumnType(column);

            // Assert
            Assert.IsFalse(actual["unicode"].GetValue<bool>());
            Assert.AreEqual("Custom",
                actual["characterSet"].GetValue<string>(),
                StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a column with explicit native type metadata.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="nativeType">The database type.</param>
        /// <param name="provider">The database provider.</param>
        /// <returns>The source column.</returns>
        private static ColumnInfo CreateColumn(Type type,
            string nativeType,
            string provider) =>
            new ColumnInfo
            {
                Field = new DbField("Value",
                    false,
                    false,
                    true,
                    type,
                    50,
                    null,
                    null,
                    nativeType,
                    provider: provider)
            };

        /// <summary>
        /// Extracts the portable type metadata of one column.
        /// </summary>
        /// <param name="column">The source column.</param>
        /// <returns>The portable column type.</returns>
        private static JsonNode ColumnType(ColumnInfo column)
        {
            var table = new TableSchema("Items",
                "dbo");
            table.Columns.Add(column);
            var definition = SchemaDefinition.Create(table.Table,
                new List<TableSchema> { table },
                RelatedTableBehavior.TableOnly);
            return definition["tables"][0]["columns"][0]["type"];
        }

        #endregion
    }
}
