#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorNamesTest
    {
        #region Split

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => AuroraDbMySqlConnectorSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = AuroraDbMySqlConnectorSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = AuroraDbMySqlConnectorSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = AuroraDbMySqlConnectorSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = AuroraDbMySqlConnectorSchemaHelper.ParseSchemaAndTable("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuote()
        {
            Assert.AreEqual("`Person`", AuroraDbMySqlConnectorSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", AuroraDbMySqlConnectorSchemaHelper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", AuroraDbMySqlConnectorSchemaHelper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", AuroraDbMySqlConnectorSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", AuroraDbMySqlConnectorSchemaHelper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", AuroraDbMySqlConnectorSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", AuroraDbMySqlConnectorSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", AuroraDbMySqlConnectorSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", AuroraDbMySqlConnectorSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird`Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = AuroraDbMySqlConnectorSchemaHelper.ParseSchemaAndTable(AuroraDbMySqlConnectorSchemaHelper.FormatTableName(schema, table));

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
