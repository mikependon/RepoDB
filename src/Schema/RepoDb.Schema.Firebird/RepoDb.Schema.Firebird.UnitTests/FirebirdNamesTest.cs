#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Firebird.UnitTests
{
    [TestClass]
    public class FirebirdNamesTest
    {
        #region Split

        [TestMethod]
        public void TestFirebirdNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, FirebirdSchemaHelper.SplitNameIntoParts("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, FirebirdSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, FirebirdSchemaHelper.SplitNameIntoParts("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, FirebirdSchemaHelper.SplitNameIntoParts("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, FirebirdSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, FirebirdSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, FirebirdSchemaHelper.SplitNameIntoParts("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestFirebirdNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, FirebirdSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => FirebirdSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => FirebirdSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => FirebirdSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestFirebirdNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = FirebirdSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = FirebirdSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = FirebirdSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = FirebirdSchemaHelper.ParseSchemaAndTable("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestFirebirdNamesQuote()
        {
            Assert.AreEqual("\"Person\"", FirebirdSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", FirebirdSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", FirebirdSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", FirebirdSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", FirebirdSchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", FirebirdSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestFirebirdNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("Person", FirebirdSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Invoice_2", FirebirdSchemaHelper.FormatTableName(null, "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesFormatWithBlankSchema()
        {
            Assert.AreEqual("Person", FirebirdSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", FirebirdSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("\"Odd.Name\"", FirebirdSchemaHelper.FormatTableName(null, "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", FirebirdSchemaHelper.FormatTableName(null, "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("\"Weird\"\"Name\"", FirebirdSchemaHelper.FormatTableName(null, "Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("\"1Table\"", FirebirdSchemaHelper.FormatTableName(null, "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdNamesFormatCanBeParsedBack()
        {
            foreach (var table in new[] { "Person", "Odd.Name", "Order Details", "Weird\"Name", "1Table", "c.d" })
            {
                // Act
                var (parsedSchema, parsedTable) = FirebirdSchemaHelper.ParseSchemaAndTable(FirebirdSchemaHelper.FormatTableName(null, table));

                // Assert
                Assert.IsNull(parsedSchema);
                Assert.AreEqual(table, parsedTable, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdNamesFormatIfTheSchemaIsGiven()
        {
            // Act/Assert
            Assert.Throws<NotSupportedException>(() => FirebirdSchemaHelper.FormatTableName("dbo", "Person"));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdNamesEnsureNoSchemaIfTheSchemaIsGiven()
        {
            // Act/Assert
            Assert.Throws<NotSupportedException>(() => FirebirdSchemaHelper.EnsureNoSchema("Sales"));
        }

        [TestMethod]
        public void TestFirebirdNamesEnsureNoSchemaWithoutSchema()
        {
            // Act/Assert
            Assert.IsNull(FirebirdSchemaHelper.EnsureNoSchema(null));
            Assert.IsNull(FirebirdSchemaHelper.EnsureNoSchema("  "));
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
