#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    [TestClass]
    public class MariaDbConnectorNamesTest
    {
        #region Split

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MariaDbConnectorSchemaHelper.SplitNameIntoParts("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => MariaDbConnectorSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => MariaDbConnectorSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => MariaDbConnectorSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestMariaDbConnectorNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = MariaDbConnectorSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = MariaDbConnectorSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = MariaDbConnectorSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = MariaDbConnectorSchemaHelper.ParseSchemaAndTable("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestMariaDbConnectorNamesQuote()
        {
            Assert.AreEqual("`Person`", MariaDbConnectorSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", MariaDbConnectorSchemaHelper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", MariaDbConnectorSchemaHelper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", MariaDbConnectorSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", MariaDbConnectorSchemaHelper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", MariaDbConnectorSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestMariaDbConnectorNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", MariaDbConnectorSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", MariaDbConnectorSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", MariaDbConnectorSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", MariaDbConnectorSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", MariaDbConnectorSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", MariaDbConnectorSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", MariaDbConnectorSchemaHelper.FormatTableName("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", MariaDbConnectorSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", MariaDbConnectorSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird`Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = MariaDbConnectorSchemaHelper.ParseSchemaAndTable(MariaDbConnectorSchemaHelper.FormatTableName(schema, table));

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
