#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Sqlite.UnitTests
{
    [TestClass]
    public class SqliteNamesTest
    {
        #region Split

        [TestMethod]
        public void TestSqliteNamesSplitOfSinglePart()
        {
            CollectionAssert.AreEqual(new[] { "Person" }, SqliteSchemaHelper.Split("Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitOfTwoParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqliteSchemaHelper.Split("dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitOfThreeParts()
        {
            CollectionAssert.AreEqual(new[] { "db", "dbo", "Person" }, SqliteSchemaHelper.Split("db.dbo.Person").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitOfBacktickedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqliteSchemaHelper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitOfQuotedParts()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqliteSchemaHelper.Split("\"dbo\".\"Person\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitKeepsTheDotOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqliteSchemaHelper.Split("\"dbo\".\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitKeepsTheDotOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqliteSchemaHelper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitKeepsTheSpaceOfABacktickedPart()
        {
            CollectionAssert.AreEqual(new[] { "Order Details" }, SqliteSchemaHelper.Split("\"Order Details\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitUnescapesTheBacktick()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Weird\"Name" }, SqliteSchemaHelper.Split("\"dbo\".\"Weird\"\"Name\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitUnescapesTheDoubleQuote()
        {
            CollectionAssert.AreEqual(new[] { "Say \"Hi\"" }, SqliteSchemaHelper.Split("\"Say \"\"Hi\"\"\"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitTrimsThePartsThatAreNotQuoted()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Person" }, SqliteSchemaHelper.Split("  dbo . Person  ").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitKeepsTheWhiteSpaceOfAQuotedPart()
        {
            CollectionAssert.AreEqual(new[] { " padded " }, SqliteSchemaHelper.Split("\" padded \"").ToArrayOf());
        }

        [TestMethod]
        public void TestSqliteNamesSplitOfMixedQuoting()
        {
            CollectionAssert.AreEqual(new[] { "dbo", "Odd.Name" }, SqliteSchemaHelper.Split("dbo.\"Odd.Name\"").ToArrayOf());
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteNamesSplitIfTheNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => SqliteSchemaHelper.Split(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteNamesSplitIfTheNameIsWhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() => SqliteSchemaHelper.Split("   "));
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteNamesSplitIfTheNameHasNoParts()
        {
            Assert.Throws<ArgumentNullException>(() => SqliteSchemaHelper.Split("..."));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestSqliteNamesParseOfTableOnly()
        {
            // Act
            var (schema, table) = SqliteSchemaHelper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesParseOfSchemaAndTable()
        {
            // Act
            var (schema, table) = SqliteSchemaHelper.Parse("Sales.Invoice");

            // Assert
            Assert.AreEqual("Sales", schema, StringComparer.Ordinal);
            Assert.AreEqual("Invoice", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesParseUsesTheLastTwoPartsOfAFullName()
        {
            // Act
            var (schema, table) = SqliteSchemaHelper.Parse("db.dbo.Person");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Person", table, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesParseOfNameWithADot()
        {
            // Act
            var (schema, table) = SqliteSchemaHelper.Parse("\"dbo\".\"Odd.Name\"");

            // Assert
            Assert.AreEqual("dbo", schema, StringComparer.Ordinal);
            Assert.AreEqual("Odd.Name", table, StringComparer.Ordinal);
        }

        #endregion

        #region Quote

        [TestMethod]
        public void TestSqliteNamesQuote()
        {
            Assert.AreEqual("\"Person\"", SqliteSchemaHelper.Quote("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesQuoteEscapesTheBacktick()
        {
            Assert.AreEqual("\"Weird\"\"Name\"", SqliteSchemaHelper.Quote("Weird\"Name"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesQuoteKeepsTheBacktickAndTheDot()
        {
            Assert.AreEqual("\"a\"\"b.c\"", SqliteSchemaHelper.Quote("a\"b.c"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesQuoteName()
        {
            Assert.AreEqual("\"dbo\".\"Person\"", SqliteSchemaHelper.QuoteName("dbo.Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesQuoteNameOfNameThatIsAlreadyQuoted()
        {
            Assert.AreEqual("\"dbo\".\"Odd.Name\"", SqliteSchemaHelper.QuoteName("\"dbo\".\"Odd.Name\""), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesQuoteNameOfSinglePart()
        {
            Assert.AreEqual("\"Person\"", SqliteSchemaHelper.QuoteName("Person"), StringComparer.Ordinal);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestSqliteNamesFormatKeepsThePlainNames()
        {
            Assert.AreEqual("dbo.Person", SqliteSchemaHelper.Format("dbo", "Person"), StringComparer.Ordinal);
            Assert.AreEqual("Sales.Invoice_2", SqliteSchemaHelper.Format("Sales", "Invoice_2"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesFormatWithoutSchema()
        {
            Assert.AreEqual("Person", SqliteSchemaHelper.Format(null, "Person"), StringComparer.Ordinal);
            Assert.AreEqual("\"Order Details\"", SqliteSchemaHelper.Format(" ", "Order Details"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesFormatQuotesTheNamesThatAreNotPlain()
        {
            Assert.AreEqual("dbo.\"Odd.Name\"", SqliteSchemaHelper.Format("dbo", "Odd.Name"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Order Details\"", SqliteSchemaHelper.Format("dbo", "Order Details"), StringComparer.Ordinal);
            Assert.AreEqual("dbo.\"Weird\"\"Name\"", SqliteSchemaHelper.Format("dbo", "Weird\"Name"), StringComparer.Ordinal);
            Assert.AreEqual("\"My Schema\".Person", SqliteSchemaHelper.Format("My Schema", "Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesFormatQuotesTheNameThatStartsWithADigit()
        {
            Assert.AreEqual("dbo.\"1Table\"", SqliteSchemaHelper.Format("dbo", "1Table"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteNamesFormatCanBeParsedBack()
        {
            foreach (var (schema, table) in new[]
            {
                ("dbo", "Person"), ("dbo", "Odd.Name"), ("My Schema", "Order Details"), ("dbo", "Weird\"Name"), ("dbo", "1Table"), ("a.b", "c.d")
            })
            {
                // Act
                var (parsedSchema, parsedTable) = SqliteSchemaHelper.Parse(SqliteSchemaHelper.Format(schema, table));

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
