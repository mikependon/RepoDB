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
            CollectionAssert.AreEqual(new[] { "Person" }, Helper.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, Helper.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, Helper.Split("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, Helper.Split("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, Helper.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, Helper.Split("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlConnectorNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = Helper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = Helper.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = Helper.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = Helper.Parse("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestMySqlConnectorNamesQuote()
        {
            Assert.AreEqual("`Person`", Helper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", Helper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", Helper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", Helper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", Helper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", Helper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestMySqlConnectorNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", Helper.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", Helper.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", Helper.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", Helper.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", Helper.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", Helper.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", Helper.Format("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", Helper.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", Helper.Format("dbo", "1Table"), StringComparer.Ordinal);
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
                var (parsedSchema, parsedTable) = Helper.Parse(Helper.Format(schema, table));

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
