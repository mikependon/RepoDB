#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Oracle.UnitTests
{
    [TestClass]
    public class OracleNamesTest
    {
        #region Split

        [TestMethod]
        public void TestOracleNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, OracleSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, OracleSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, OracleSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, OracleSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, OracleSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, OracleSchemaHelper.SplitNameIntoParts("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, OracleSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, OracleSchemaHelper.SplitNameIntoParts("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, OracleSchemaHelper.SplitNameIntoParts("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, OracleSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, OracleSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, OracleSchemaHelper.SplitNameIntoParts("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestOracleNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, OracleSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnOracleNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => OracleSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => OracleSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => OracleSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestOracleNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestOracleNamesQuote()
        {
            Assert.AreEqual("\"Person\"", OracleSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", OracleSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", OracleSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", OracleSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", OracleSchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", OracleSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestOracleNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", OracleSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", OracleSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", OracleSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", OracleSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.\"Odd.Name\"", OracleSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Order Details\"", OracleSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Weird\"\"Name\"", OracleSchemaHelper.FormatTableName("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"My Schema\".Person", OracleSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.\"1Table\"", OracleSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = OracleSchemaHelper.ParseSchemaAndTable(OracleSchemaHelper.FormatTableName(schema, table));

                // Assert
                Assert.AreEqual(schema, parsedSchema, StringComparer.Ordinal);
                Assert.AreEqual(table, parsedTable, StringComparer.Ordinal);
            }
        }

        #endregion
    }

    internal static class NamesTestExtensions
    {
        public static string[] ToArrayOf(this System.Collections.Generic.IList<string> list)
        {
            var array = new string[list.Count];
            list.CopyTo(array, 0);
            return array;
        }
    }
}
