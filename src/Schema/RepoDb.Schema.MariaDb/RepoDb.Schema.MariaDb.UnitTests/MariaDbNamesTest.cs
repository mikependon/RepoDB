#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    [TestClass]
    public class MariaDbNamesTest
    {
        #region Split

        [TestMethod]
        public void TestMariaDbNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, Helper.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, Helper.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("`dbo`.`Person`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("`dbo`.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, Helper.Split("`Order Details`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird`Name" }, Helper.Split("`dbo`.`Weird``Name`").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, Helper.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, Helper.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, Helper.Split("` padded `").ToArrayOf());
        }

        [TestMethod]
        public void TestMariaDbNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, Helper.Split("dbo.`Odd.Name`").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => Helper.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestMariaDbNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = Helper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = Helper.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = Helper.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesParseOfNameWithADot()
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
        public void TestMariaDbNamesQuote()
        {
            Assert.AreEqual("`Person`", Helper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("`Weird``Name`", Helper.Quote("Weird`Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("`a``b.c`", Helper.Quote("a`b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesQuoteName()
        {
            Assert.AreEqual("`dbo`.`Person`", Helper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("`dbo`.`Odd.Name`", Helper.QuoteName("`dbo`.`Odd.Name`"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("`Person`", Helper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestMariaDbNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", Helper.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", Helper.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", Helper.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("`Order Details`", Helper.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.`Odd.Name`", Helper.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Order Details`", Helper.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.`Weird``Name`", Helper.Format("dbo", "Weird`Name"), StringComparer.Ordinal);
            Assert.AreEqual("`My Schema`.Person", Helper.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.`1Table`", Helper.Format("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbNamesFormatCanBeParsedBack()
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
