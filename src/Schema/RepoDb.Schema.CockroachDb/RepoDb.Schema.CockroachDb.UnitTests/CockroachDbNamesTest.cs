#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.CockroachDb.UnitTests
{
    [TestClass]
    public class HelperTest
    {
        #region Split / Parse

        [TestMethod]
        public void TestHelperParseTableOnly()
        {
            // Act
            var (schema, table) = CockroachDbSchemaHelper.Parse("Person");

            // Assert
            Assert.IsNull(schema);
            Assert.AreEqual("Person", table);
        }

        [TestMethod]
        public void TestHelperParseSchemaAndTable()
        {
            // Act
            var (schema, table) = CockroachDbSchemaHelper.Parse("sales.order_line");

            // Assert
            Assert.AreEqual("sales", schema);
            Assert.AreEqual("order_line", table);
        }

        [TestMethod]
        public void TestHelperParseQuotedParts()
        {
            // Act
            var (schema, table) = CockroachDbSchemaHelper.Parse("\"my.schema\".\"Order \"\"Details\"\"\"");

            // Assert
            Assert.AreEqual("my.schema", schema);
            Assert.AreEqual("Order \"Details\"", table);
        }

        [TestMethod]
        public void TestHelperParseUsesTheLastTwoParts()
        {
            // Act
            var (schema, table) = CockroachDbSchemaHelper.Parse("db.sales.order_line");

            // Assert
            Assert.AreEqual("sales", schema);
            Assert.AreEqual("order_line", table);
        }

        [TestMethod]
        public void ThrowExceptionOnHelperSplitIfTheNameIsBlank()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => CockroachDbSchemaHelper.Split(null));
            Assert.Throws<ArgumentNullException>(() => CockroachDbSchemaHelper.Split(" "));
        }

        #endregion

        #region Quote / Format

        [TestMethod]
        public void TestHelperQuote()
        {
            // Assert
            Assert.AreEqual("\"Person\"", CockroachDbSchemaHelper.Quote("Person"));
            Assert.AreEqual("\"a\"\"b\"", CockroachDbSchemaHelper.Quote("a\"b"));
        }

        [TestMethod]
        public void TestHelperQuoteName()
        {
            // Assert
            Assert.AreEqual("\"public\".\"Person\"", CockroachDbSchemaHelper.QuoteName("public.Person"));
            Assert.AreEqual("\"Person\"", CockroachDbSchemaHelper.QuoteName("Person"));
        }

        [TestMethod]
        public void TestHelperFormatKeepsThePlainLowerCaseNames()
        {
            // Assert
            Assert.AreEqual("public.person", CockroachDbSchemaHelper.Format("public", "person"));
            Assert.AreEqual("person", CockroachDbSchemaHelper.Format(null, "person"));
        }

        [TestMethod]
        public void TestHelperFormatQuotesTheOtherNames()
        {
            // Assert
            Assert.AreEqual("public.\"Person\"", CockroachDbSchemaHelper.Format("public", "Person"));
            Assert.AreEqual("\"My Schema\".\"1table\"", CockroachDbSchemaHelper.Format("My Schema", "1table"));
        }

        [TestMethod]
        public void TestHelperFormatCanBeParsedBack()
        {
            // Act
            var (schema, table) = CockroachDbSchemaHelper.Parse(CockroachDbSchemaHelper.Format("My.Schema", "Order \"Details\""));

            // Assert
            Assert.AreEqual("My.Schema", schema);
            Assert.AreEqual("Order \"Details\"", table);
        }

        #endregion
    }
}
