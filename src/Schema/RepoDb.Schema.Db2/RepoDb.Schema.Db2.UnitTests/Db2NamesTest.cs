#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Db2.UnitTests
{
    [TestClass]
    public class Db2NamesTest
    {
        #region Split

        [TestMethod]
        public void TestDb2NamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, Db2SchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Db2SchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, Db2SchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Db2SchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Db2SchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Db2SchemaHelper.SplitNameIntoParts("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Db2SchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, Db2SchemaHelper.SplitNameIntoParts("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, Db2SchemaHelper.SplitNameIntoParts("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, Db2SchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Db2SchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, Db2SchemaHelper.SplitNameIntoParts("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestDb2NamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Db2SchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnDb2NamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Db2SchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2NamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => Db2SchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2NamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => Db2SchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestDb2NamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestDb2NamesQuote()
        {
            Assert.AreEqual("\"Person\"", Db2SchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", Db2SchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", Db2SchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", Db2SchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", Db2SchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", Db2SchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestDb2NamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", Db2SchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", Db2SchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", Db2SchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", Db2SchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.\"Odd.Name\"", Db2SchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Order Details\"", Db2SchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Weird\"\"Name\"", Db2SchemaHelper.FormatTableName("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"My Schema\".Person", Db2SchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.\"1Table\"", Db2SchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2NamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = Db2SchemaHelper.ParseSchemaAndTable(Db2SchemaHelper.FormatTableName(schema, table));

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
