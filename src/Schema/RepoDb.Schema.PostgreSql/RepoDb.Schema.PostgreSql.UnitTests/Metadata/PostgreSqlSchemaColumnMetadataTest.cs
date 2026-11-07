#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.PostgreSql.UnitTests.Metadata
{
    [TestClass]
    public class PostgreSqlSchemaColumnMetadataTest
    {
        #region Private Methods

        private static ColumnInfo CreateColumn(string characterSet,
            bool? unicode) =>
            new ColumnInfo
            {
                CharacterSet = characterSet,
                IsUnicode = unicode
            };

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestApplyLeavesTheEncodingUnknownForSqlAscii()
        {
            // Act
            var actual = PostgreSqlSchemaColumnMetadata.Apply(CreateColumn("SQL_ASCII",
                false));

            // Assert
            Assert.IsNull(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyKeepsTheEncodingOfAnotherCharacterSet()
        {
            // Act
            var actual = PostgreSqlSchemaColumnMetadata.Apply(CreateColumn("UTF8",
                true));

            // Assert
            Assert.IsTrue(actual.IsUnicode);
        }

        [TestMethod]
        public void TestApplyKeepsAColumnWithoutACharacterSet()
        {
            // Act
            var actual = PostgreSqlSchemaColumnMetadata.Apply(CreateColumn(null,
                true));

            // Assert
            Assert.IsTrue(actual.IsUnicode);
        }

        #endregion
    }
}
