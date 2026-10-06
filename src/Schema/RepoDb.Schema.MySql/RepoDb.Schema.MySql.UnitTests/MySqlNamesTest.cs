#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.MySql.UnitTests
{
    [TestClass]
    public class MySqlNamesTest
    {
        #region Split

        [TestMethod]
        public void TestMySqlNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, MySqlSchemaHelper.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlSchemaHelper.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, MySqlSchemaHelper.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlSchemaHelper.Split("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlSchemaHelper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlSchemaHelper.Split("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlSchemaHelper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, MySqlSchemaHelper.Split("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, MySqlSchemaHelper.Split("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, MySqlSchemaHelper.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, MySqlSchemaHelper.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, MySqlSchemaHelper.Split("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestMySqlNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, MySqlSchemaHelper.Split("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlSchemaHelper.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlSchemaHelper.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => MySqlSchemaHelper.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestMySqlNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = MySqlSchemaHelper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = MySqlSchemaHelper.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = MySqlSchemaHelper.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = MySqlSchemaHelper.Parse("`dbo`.`Odd.Name`");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestMySqlNamesQuote()
        {
            Assert.AreEqual("`Person`", MySqlSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", MySqlSchemaHelper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", MySqlSchemaHelper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", MySqlSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", MySqlSchemaHelper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", MySqlSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestMySqlNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", MySqlSchemaHelper.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", MySqlSchemaHelper.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", MySqlSchemaHelper.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", MySqlSchemaHelper.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", MySqlSchemaHelper.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", MySqlSchemaHelper.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", MySqlSchemaHelper.Format("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", MySqlSchemaHelper.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", MySqlSchemaHelper.Format("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird`Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = MySqlSchemaHelper.Parse(MySqlSchemaHelper.Format(schema, table));

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
