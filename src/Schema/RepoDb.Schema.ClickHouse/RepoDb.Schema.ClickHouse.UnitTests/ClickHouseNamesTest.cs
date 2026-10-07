#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.ClickHouse.UnitTests
{
    [TestClass]
    public class ClickHouseNamesTest
    {
        #region Split

        [TestMethod]
        public void TestClickHouseNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, ClickHouseSchemaHelper.SplitNameIntoParts("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, ClickHouseSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, ClickHouseSchemaHelper.SplitNameIntoParts("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, ClickHouseSchemaHelper.SplitNameIntoParts("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, ClickHouseSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, ClickHouseSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, ClickHouseSchemaHelper.SplitNameIntoParts("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestClickHouseNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, ClickHouseSchemaHelper.SplitNameIntoParts("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ClickHouseSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => ClickHouseSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => ClickHouseSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestClickHouseNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestClickHouseNamesQuote()
        {
            Assert.AreEqual("`Person`", ClickHouseSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", ClickHouseSchemaHelper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", ClickHouseSchemaHelper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", ClickHouseSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", ClickHouseSchemaHelper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", ClickHouseSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestClickHouseNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", ClickHouseSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", ClickHouseSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", ClickHouseSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", ClickHouseSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", ClickHouseSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", ClickHouseSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", ClickHouseSchemaHelper.FormatTableName("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", ClickHouseSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", ClickHouseSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird`Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = ClickHouseSchemaHelper.ParseSchemaAndTable(ClickHouseSchemaHelper.FormatTableName(schema, table));

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
