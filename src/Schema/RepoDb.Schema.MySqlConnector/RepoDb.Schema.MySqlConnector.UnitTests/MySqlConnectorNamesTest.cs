#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.MySqlConnector.UnitTests
{
    [TestClass]
    public class MySqlConnectorNamesTest
    {
        #region Split

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, MySqlConnectorSchemaHelper.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlConnectorSchemaHelper.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, MySqlConnectorSchemaHelper.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlConnectorSchemaHelper.Split("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlConnectorSchemaHelper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlConnectorSchemaHelper.Split("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlConnectorSchemaHelper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, MySqlConnectorSchemaHelper.Split("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, MySqlConnectorSchemaHelper.Split("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, MySqlConnectorSchemaHelper.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlConnectorSchemaHelper.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, MySqlConnectorSchemaHelper.Split("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlConnectorSchemaHelper.Split("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlConnectorSchemaHelper.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlConnectorSchemaHelper.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlConnectorSchemaHelper.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = MySqlConnectorSchemaHelper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = MySqlConnectorSchemaHelper.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = MySqlConnectorSchemaHelper.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = MySqlConnectorSchemaHelper.Parse("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestMySqlConnectorNamesQuote()
        {
            Assert.AreEqual("`Person`", MySqlConnectorSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", MySqlConnectorSchemaHelper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", MySqlConnectorSchemaHelper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", MySqlConnectorSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", MySqlConnectorSchemaHelper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", MySqlConnectorSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestMySqlConnectorNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", MySqlConnectorSchemaHelper.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", MySqlConnectorSchemaHelper.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", MySqlConnectorSchemaHelper.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", MySqlConnectorSchemaHelper.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", MySqlConnectorSchemaHelper.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", MySqlConnectorSchemaHelper.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", MySqlConnectorSchemaHelper.Format("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", MySqlConnectorSchemaHelper.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", MySqlConnectorSchemaHelper.Format("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird`Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = MySqlConnectorSchemaHelper.Parse(MySqlConnectorSchemaHelper.Format(schema, table));

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
