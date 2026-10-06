#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    [TestClass]
    public class SqlServerNamesTest
    {
        #region Split

        [TestMethod]
        public void TestSqlServerNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfBracketedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("[dbo].[Person]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheDotOfABracketedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerSchemaHelper.SplitNameIntoParts("[dbo].[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerSchemaHelper.SplitNameIntoParts("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheSpaceOfABracketedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, SqlServerSchemaHelper.SplitNameIntoParts("[Order Details]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitUnescapesTheClosingBracket()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird]Name" }, SqlServerSchemaHelper.SplitNameIntoParts("[dbo].[Weird]]Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, SqlServerSchemaHelper.SplitNameIntoParts("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerSchemaHelper.SplitNameIntoParts("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, SqlServerSchemaHelper.SplitNameIntoParts("[ padded ]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerSchemaHelper.SplitNameIntoParts("dbo.[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerSchemaHelper.SplitNameIntoParts(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerSchemaHelper.SplitNameIntoParts("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerSchemaHelper.SplitNameIntoParts("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestSqlServerNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = SqlServerSchemaHelper.ParseSchemaAndTable("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = SqlServerSchemaHelper.ParseSchemaAndTable("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = SqlServerSchemaHelper.ParseSchemaAndTable("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = SqlServerSchemaHelper.ParseSchemaAndTable("[dbo].[Odd.Name]");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestSqlServerNamesQuote()
        {
            Assert.AreEqual("[Person]", SqlServerSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteEscapesTheClosingBracket()
        {
            Assert.AreEqual("[Weird]]Name]", SqlServerSchemaHelper.Quote("Weird]Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteKeepsTheOpeningBracketAndTheDot()
        {
            Assert.AreEqual("[a[b.c]", SqlServerSchemaHelper.Quote("a[b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteName()
        {
            Assert.AreEqual("[dbo].[Person]", SqlServerSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("[dbo].[Odd.Name]", SqlServerSchemaHelper.QuoteName("[dbo].[Odd.Name]"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("[Person]", SqlServerSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestSqlServerNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", SqlServerSchemaHelper.FormatTableName("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", SqlServerSchemaHelper.FormatTableName("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", SqlServerSchemaHelper.FormatTableName(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("[Order Details]", SqlServerSchemaHelper.FormatTableName(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.[Odd.Name]", SqlServerSchemaHelper.FormatTableName("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Order Details]", SqlServerSchemaHelper.FormatTableName("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Weird]]Name]", SqlServerSchemaHelper.FormatTableName("dbo", "Weird]Name"), StringComparer.Ordinal);
            Assert.AreEqual("[My Schema].Person", SqlServerSchemaHelper.FormatTableName("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.[1Table]", SqlServerSchemaHelper.FormatTableName("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird]Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = SqlServerSchemaHelper.ParseSchemaAndTable(SqlServerSchemaHelper.FormatTableName(schema, table));

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
