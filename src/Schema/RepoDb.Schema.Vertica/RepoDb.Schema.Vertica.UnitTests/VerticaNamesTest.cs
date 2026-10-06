#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Vertica.UnitTests
{
    [TestClass]
    public class VerticaNamesTest
    {
        #region Split

        [TestMethod]
        public void TestVerticaNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, VerticaSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, VerticaSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, VerticaSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, VerticaSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, VerticaSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, VerticaSchemaHelper.SplitNameIntoParts("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, VerticaSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, VerticaSchemaHelper.SplitNameIntoParts("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, VerticaSchemaHelper.SplitNameIntoParts("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, VerticaSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, VerticaSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, VerticaSchemaHelper.SplitNameIntoParts("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestVerticaNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, VerticaSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => VerticaSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => VerticaSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => VerticaSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestVerticaNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestVerticaNamesQuote()
        {
            Assert.AreEqual("\"Person\"", VerticaSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", VerticaSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", VerticaSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", VerticaSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", VerticaSchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", VerticaSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestVerticaNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", VerticaSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", VerticaSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", VerticaSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", VerticaSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.\"Odd.Name\"", VerticaSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Order Details\"", VerticaSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Weird\"\"Name\"", VerticaSchemaHelper.FormatTableName("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"My Schema\".Person", VerticaSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.\"1Table\"", VerticaSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = VerticaSchemaHelper.ParseSchemaAndTable(VerticaSchemaHelper.FormatTableName(schema, table));

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
