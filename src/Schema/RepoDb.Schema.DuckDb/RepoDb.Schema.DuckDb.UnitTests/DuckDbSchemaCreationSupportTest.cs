#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.DuckDb.UnitTests
{
    [TestClass]
    public class DuckDbSchemaCreationSupportTest
    {
        #region Private Variables

        private readonly DuckDbSchemaComposer _composer = new DuckDbSchemaComposer();

        #endregion

        #region Properties

        [TestMethod]
        public void TestComposerSupportsTheSchemaCreation()
        {
            // Act/Assert
            Assert.IsInstanceOfType<ISchemaCreationSupport>(_composer);
        }

        [TestMethod]
        public void TestCreateSchemaSqlIsTheCreateStatementOfTheProvider()
        {
            // Act
            var actual = _composer.CreateSchemaSql;

            // Assert
            Assert.AreEqual(DuckDbSchemaText.CreateSchemaSql, actual);
        }

        [TestMethod]
        public void TestSchemaExistsSqlIsTheCheckStatementOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaExistsSql;

            // Assert
            Assert.AreEqual(DuckDbSchemaText.SchemaExistsSql, actual);
        }

        [TestMethod]
        public void TestSchemaParameterPrefixIsThePrefixOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaParameterPrefix;

            // Assert
            Assert.AreEqual("$", actual);
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestQuoteSchemaNameQuotesTheNameInTheDialectOfTheProvider()
        {
            // Act
            var actual = _composer.QuoteSchemaName("Sales");

            // Assert
            Assert.AreEqual("\"Sales\"", actual);
        }

        #endregion
    }
}
