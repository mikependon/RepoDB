#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class SchemaIdentifierQuoteTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaIdentifierQuoteOpenOfAPair()
        {
            // Act
            var quote = new SchemaIdentifierQuote('[',
                ']');

            // Assert
            Assert.AreEqual('[', quote.Open);
        }

        [TestMethod]
        public void TestSchemaIdentifierQuoteCloseOfAPair()
        {
            // Act
            var quote = new SchemaIdentifierQuote('[',
                ']');

            // Assert
            Assert.AreEqual(']', quote.Close);
        }

        [TestMethod]
        public void TestSchemaIdentifierQuoteOpenOfASingleQuote()
        {
            // Act
            var quote = new SchemaIdentifierQuote('"');

            // Assert
            Assert.AreEqual('"', quote.Open);
        }

        [TestMethod]
        public void TestSchemaIdentifierQuoteCloseOfASingleQuote()
        {
            // Act
            var quote = new SchemaIdentifierQuote('"');

            // Assert
            Assert.AreEqual('"', quote.Close);
        }

        #endregion
    }
}
