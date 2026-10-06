#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.DuckDb.UnitTests
{
    [TestClass]
    public class DuckDbNamesTest
    {
        #region Split

        [TestMethod]
        public void TestDuckDbNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, DuckDbSchemaHelper.SplitNameIntoParts("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, DuckDbSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, DuckDbSchemaHelper.SplitNameIntoParts("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, DuckDbSchemaHelper.SplitNameIntoParts("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, DuckDbSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, DuckDbSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, DuckDbSchemaHelper.SplitNameIntoParts("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestDuckDbNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, DuckDbSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => DuckDbSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => DuckDbSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => DuckDbSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestDuckDbNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestDuckDbNamesQuote()
        {
            Assert.AreEqual("\"Person\"", DuckDbSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", DuckDbSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", DuckDbSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", DuckDbSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", DuckDbSchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", DuckDbSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestDuckDbNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", DuckDbSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", DuckDbSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", DuckDbSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", DuckDbSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.\"Odd.Name\"", DuckDbSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Order Details\"", DuckDbSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Weird\"\"Name\"", DuckDbSchemaHelper.FormatTableName("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"My Schema\".Person", DuckDbSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.\"1Table\"", DuckDbSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = DuckDbSchemaHelper.ParseSchemaAndTable(DuckDbSchemaHelper.FormatTableName(schema, table));

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
