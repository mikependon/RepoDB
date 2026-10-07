#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Ahtola.UnitTests
{
    [TestClass]
    public class AhtolaNamesTest
    {
        #region Split

        [TestMethod]
        public void TestAhtolaNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("[dbo].[Person]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("[dbo].[Person]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AhtolaSchemaHelper.SplitNameIntoParts("[dbo].[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AhtolaSchemaHelper.SplitNameIntoParts("dbo.[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, AhtolaSchemaHelper.SplitNameIntoParts("[Order Details]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, AhtolaSchemaHelper.SplitNameIntoParts("[dbo].[Weird\"Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, AhtolaSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, AhtolaSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, AhtolaSchemaHelper.SplitNameIntoParts("[ padded ]").ToArrayOf());
        }

        [TestMethod]
        public void TestAhtolaNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, AhtolaSchemaHelper.SplitNameIntoParts("dbo.[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnAhtolaNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => AhtolaSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnAhtolaNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => AhtolaSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnAhtolaNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => AhtolaSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestAhtolaNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = AhtolaSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = AhtolaSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = AhtolaSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = AhtolaSchemaHelper.ParseSchemaAndTable("[dbo].[Odd.Name]");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestAhtolaNamesQuote()
        {
            Assert.AreEqual("[Person]", AhtolaSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("[Weird\"Name]", AhtolaSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("[a\"b.c]", AhtolaSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesQuoteName()
        {
            Assert.AreEqual("[dbo].[Person]", AhtolaSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("[dbo].[Odd.Name]", AhtolaSchemaHelper.QuoteName("[dbo].[Odd.Name]"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("[Person]", AhtolaSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestAhtolaNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", AhtolaSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", AhtolaSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", AhtolaSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("[Order Details]", AhtolaSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.[Odd.Name]", AhtolaSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Order Details]", AhtolaSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Weird\"Name]", AhtolaSchemaHelper.FormatTableName("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("[My Schema].Person", AhtolaSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.[1Table]", AhtolaSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = AhtolaSchemaHelper.ParseSchemaAndTable(AhtolaSchemaHelper.FormatTableName(schema, table));

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
