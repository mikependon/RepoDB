#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.SqlServer.UnitTests.Metadata
{
    [TestClass]
    public class SqlServerSchemaColumnMetadataTest
    {
        #region Private Methods

        private static ColumnInfo Apply(string nativeType,
            object codePage,
            string collation = null,
            Type type = null,
            byte? scale = null)
        {
            var data = new DataTable();
            data.Columns.Add("CodePage", typeof(object));
            data.Rows.Add(codePage);
            using var record = data.CreateDataReader();
            record.Read();
            var column = new ColumnInfo
            {
                Collation = collation,
                Field = new DbField("Name", false, false, true, type ?? typeof(string), 100, null, scale, nativeType, false, "MSSQL")
            };
            return SqlServerSchemaColumnMetadata.Apply(column,
                record);
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestApplyAUtf8CodePageIsUnicode()
        {
            // Act
            var actual = Apply("varchar",
                65001);

            // Assert
            Assert.IsTrue(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyAUtf8CodePageHasTheUtf8CharacterSet()
        {
            // Act
            var actual = Apply("varchar",
                65001);

            // Assert
            Assert.AreEqual("UTF-8", actual.CharacterSet);
        }

        [TestMethod]
        public void TestApplyALegacyCodePageIsNotUnicode()
        {
            // Act
            var actual = Apply("varchar",
                1252);

            // Assert
            Assert.IsFalse(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyALegacyCodePageHasTheCodePageAsCharacterSet()
        {
            // Act
            var actual = Apply("varchar",
                1252);

            // Assert
            Assert.AreEqual("CP1252", actual.CharacterSet);
        }

        [TestMethod]
        public void TestApplyANonUnicodeTextIsMeasuredInBytes()
        {
            // Act
            var actual = Apply("varchar",
                1252);

            // Assert
            Assert.AreEqual("bytes", actual.LengthUnit);
        }

        [TestMethod]
        public void TestApplyWithoutACodePageUsesTheUtf8Collation()
        {
            // Act
            var actual = Apply("varchar",
                DBNull.Value,
                "Latin1_General_100_CI_AS_SC_UTF8");

            // Assert
            Assert.AreEqual("UTF-8", actual.CharacterSet);
        }

        [TestMethod]
        public void TestApplyWithoutACodePageAndACollationOfALegacyEncodingKeepsItNotUnicode()
        {
            // Act
            var actual = Apply("varchar",
                DBNull.Value,
                "Latin1_General_CI_AS");

            // Assert
            Assert.IsFalse(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyANationalTextHasTheUtf16CharacterSet()
        {
            // Act
            var actual = Apply("nvarchar",
                DBNull.Value);

            // Assert
            Assert.AreEqual("UTF-16", actual.CharacterSet);
        }

        [TestMethod]
        public void TestApplyANationalTextIsUnicode()
        {
            // Act
            var actual = Apply("nvarchar",
                DBNull.Value);

            // Assert
            Assert.IsTrue(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyANationalTextIsMeasuredInUtf16CodeUnits()
        {
            // Act
            var actual = Apply("nchar",
                DBNull.Value);

            // Assert
            Assert.AreEqual("utf16CodeUnits", actual.LengthUnit);
        }

        [TestMethod]
        public void TestApplyAColumnThatIsNotTextHasNoEncoding()
        {
            // Act
            var actual = Apply("int",
                DBNull.Value,
                null,
                typeof(int));

            // Assert
            Assert.IsNull(actual.CharacterSet);
        }

        [TestMethod]
        public void TestApplyATemporalColumnHasThePrecisionOfItsScale()
        {
            // Act
            var actual = Apply("datetime2",
                DBNull.Value,
                null,
                typeof(DateTime),
                7);

            // Assert
            Assert.AreEqual(7, actual.TemporalPrecision);
        }

        #endregion
    }
}
