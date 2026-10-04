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
            CollectionAssert.AreEqual(new[] { "Person" }, SqlServerNames.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerNames.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, SqlServerNames.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfBracketedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerNames.Split("[dbo].[Person]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerNames.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheDotOfABracketedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerNames.Split("[dbo].[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerNames.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheSpaceOfABracketedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, SqlServerNames.Split("[Order Details]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitUnescapesTheClosingBracket()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird]Name" }, SqlServerNames.Split("[dbo].[Weird]]Name]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, SqlServerNames.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqlServerNames.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, SqlServerNames.Split("[ padded ]").ToArrayOf());
        }

        [TestMethod]
        public void TestSqlServerNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqlServerNames.Split("dbo.[Odd.Name]").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerNames.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerNames.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => SqlServerNames.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestSqlServerNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = SqlServerNames.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = SqlServerNames.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = SqlServerNames.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = SqlServerNames.Parse("[dbo].[Odd.Name]");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestSqlServerNamesQuote()
        {
            Assert.AreEqual("[Person]", SqlServerNames.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteEscapesTheClosingBracket()
        {
            Assert.AreEqual("[Weird]]Name]", SqlServerNames.Quote("Weird]Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteKeepsTheOpeningBracketAndTheDot()
        {
            Assert.AreEqual("[a[b.c]", SqlServerNames.Quote("a[b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteName()
        {
            Assert.AreEqual("[dbo].[Person]", SqlServerNames.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("[dbo].[Odd.Name]", SqlServerNames.QuoteName("[dbo].[Odd.Name]"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("[Person]", SqlServerNames.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestSqlServerNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", SqlServerNames.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", SqlServerNames.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", SqlServerNames.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("[Order Details]", SqlServerNames.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.[Odd.Name]", SqlServerNames.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Order Details]", SqlServerNames.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.[Weird]]Name]", SqlServerNames.Format("dbo", "Weird]Name"), StringComparer.Ordinal);
            Assert.AreEqual("[My Schema].Person", SqlServerNames.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.[1Table]", SqlServerNames.Format("dbo", "1Table"), StringComparer.Ordinal);
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
                var (parsedSchema, parsedTable) = SqlServerNames.Parse(SqlServerNames.Format(schema, table));

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
