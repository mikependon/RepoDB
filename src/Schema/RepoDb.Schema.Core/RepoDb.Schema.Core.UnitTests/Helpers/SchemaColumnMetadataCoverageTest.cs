using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using System;
using System.Data;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class SchemaColumnMetadataCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Recognizes Unicode and legacy character encodings.
        /// </summary>
        [TestMethod]
        public void ApplyRecognizesCharacterSetEncodingForUtf8Mb4()
        {
            // Setup
            string charset = "utf8mb4";
            bool expected = true;
            var column = CreateColumn("varchar");

            // Act
            var actual = Apply(column,
                "CharacterSet",
                charset);

            // Assert
            Assert.AreEqual(expected,
                actual.IsUnicode);
        }

        /// <summary>
        /// Recognizes Unicode and legacy character encodings.
        /// </summary>
        [TestMethod]
        public void ApplyRecognizesCharacterSetEncodingForUtf16InUppercase()
        {
            // Setup
            string charset = "UTF-16";
            bool expected = true;
            var column = CreateColumn("varchar");

            // Act
            var actual = Apply(column,
                "CharacterSet",
                charset);

            // Assert
            Assert.AreEqual(expected,
                actual.IsUnicode);
        }

        /// <summary>
        /// Recognizes Unicode and legacy character encodings.
        /// </summary>
        [TestMethod]
        public void ApplyRecognizesCharacterSetEncodingForUcs2()
        {
            // Setup
            string charset = "ucs2";
            bool expected = true;
            var column = CreateColumn("varchar");

            // Act
            var actual = Apply(column,
                "CharacterSet",
                charset);

            // Assert
            Assert.AreEqual(expected,
                actual.IsUnicode);
        }

        /// <summary>
        /// Recognizes Unicode and legacy character encodings.
        /// </summary>
        [TestMethod]
        public void ApplyRecognizesCharacterSetEncodingForLatin1()
        {
            // Setup
            string charset = "latin1";
            bool expected = false;
            var column = CreateColumn("varchar");

            // Act
            var actual = Apply(column,
                "CharacterSet",
                charset);

            // Assert
            Assert.AreEqual(expected,
                actual.IsUnicode);
        }

        /// <summary>
        /// Leaves ambiguous character encodings unspecified.
        /// </summary>
        [TestMethod]
        public void ApplyPreservesUnknownUnicodeForSqlAscii()
        {
            // Setup
            var column = CreateColumn("varchar");

            // Act
            var actual = Apply(column,
                "CharacterSet",
                "sql_ascii");

            // Assert
            Assert.IsNull(actual.IsUnicode);
        }

        /// <summary>
        /// Reads precision and negative scale beyond byte limits.
        /// </summary>
        [TestMethod]
        public void ApplyPreservesWideNumericMetadata()
        {
            // Setup
            var column = CreateColumn("numeric");

            // Act
            var actual = Apply(column,
                "NativeType",
                "NUMERIC(1000, -5)");

            // Assert
            Assert.AreEqual(1000,
                actual.NumericPrecision);
            Assert.AreEqual(-5,
                actual.NumericScale);
        }

        /// <summary>
        /// Preserves an optional catalog numeric value.
        /// </summary>
        [TestMethod]
        public void ApplyReadsPrecision()
        {
            // Setup
            var column = CreateColumn("numeric");

            // Act
            var actual = Apply(column,
                "Precision",
                6);

            // Assert
            Assert.AreEqual(6,
                actual.NumericPrecision);
        }

        /// <summary>
        /// Preserves an optional catalog numeric value.
        /// </summary>
        [TestMethod]
        public void ApplyReadsScale()
        {
            // Setup
            var column = CreateColumn("numeric");

            // Act
            var actual = Apply(column,
                "Scale",
                6);

            // Assert
            Assert.AreEqual(6,
                actual.NumericScale);
        }

        /// <summary>
        /// Preserves an optional catalog numeric value.
        /// </summary>
        [TestMethod]
        public void ApplyReadsTemporalPrecision()
        {
            // Setup
            var column = CreateColumn("numeric");

            // Act
            var actual = Apply(column,
                "TemporalPrecision",
                6);

            // Assert
            Assert.AreEqual(6,
                actual.TemporalPrecision);
        }

        /// <summary>
        /// Distinguishes UTF-8 from legacy SQL Server code pages.
        /// </summary>
        [TestMethod]
        public void ApplySqlServerCodePageDefinesEncodingForValue65001()
        {
            // Setup
            int codePage = 65001;
            string charset = "UTF-8";
            bool unicode = true;
            var column = CreateColumn("varchar",
                "SqlServer");

            // Act
            var actual = Apply(column,
                "CodePage",
                codePage);

            // Assert
            Assert.AreEqual(charset,
                actual.CharacterSet,
                StringComparer.Ordinal);
            Assert.AreEqual(unicode,
                actual.IsUnicode);
        }

        /// <summary>
        /// Distinguishes UTF-8 from legacy SQL Server code pages.
        /// </summary>
        [TestMethod]
        public void ApplySqlServerCodePageDefinesEncodingForValue1252()
        {
            // Setup
            int codePage = 1252;
            string charset = "CP1252";
            bool unicode = false;
            var column = CreateColumn("varchar",
                "SqlServer");

            // Act
            var actual = Apply(column,
                "CodePage",
                codePage);

            // Assert
            Assert.AreEqual(charset,
                actual.CharacterSet,
                StringComparer.Ordinal);
            Assert.AreEqual(unicode,
                actual.IsUnicode);
        }

        /// <summary>
        /// Uses collation metadata when no code page is available.
        /// </summary>
        [TestMethod]
        public void ApplySqlServerFallsBackToUtf8Collation()
        {
            // Setup
            var column = CreateColumn("varchar",
                "SqlServer");
            column.Collation = "Latin1_General_100_CI_AS_SC_UTF8";

            // Act
            var actual = Apply(column,
                "CodePage",
                DBNull.Value);

            // Assert
            Assert.AreEqual("UTF-8",
                actual.CharacterSet,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Identifies SQL Server national character columns.
        /// </summary>
        [TestMethod]
        public void ApplySqlServerNvarcharUsesUtf16()
        {
            // Setup
            var column = CreateColumn("nvarchar",
                "SqlServer");

            // Act
            var actual = Apply(column,
                "CodePage",
                DBNull.Value);

            // Assert
            Assert.AreEqual("UTF-16",
                actual.CharacterSet,
                StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a text column for a catalog metadata test.
        /// </summary>
        /// <param name="nativeType">The native database type.</param>
        /// <param name="provider">The source provider.</param>
        /// <returns>The source column.</returns>
        private static ColumnInfo CreateColumn(string nativeType,
            string provider = "Other") =>
            new ColumnInfo
            {
                Field = new DbField("Value",
                    false,
                    false,
                    true,
                    typeof(string),
                    50,
                    null,
                    null,
                    nativeType,
                    provider: provider)
            };

        /// <summary>
        /// Applies a single optional catalog field.
        /// </summary>
        /// <param name="column">The source column.</param>
        /// <param name="name">The optional metadata field.</param>
        /// <param name="value">The catalog value.</param>
        /// <returns>The enriched column.</returns>
        private static ColumnInfo Apply(ColumnInfo column,
            string name,
            object value)
        {
            using var table = new DataTable();
            table.Columns.Add(name,
                typeof(object));
            table.Rows.Add(value);
            using var record = table.CreateDataReader();
            record.Read();
            return SchemaColumnMetadata.Apply(column,
                record);
        }

        #endregion
    }
}
