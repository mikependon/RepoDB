#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.SapHana.UnitTests
{
    [TestClass]
    public class SapHanaSchemaCreationSupportTest
    {
        #region Private Variables

        private readonly SapHanaSchemaComposer _composer = new SapHanaSchemaComposer();

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
            Assert.AreEqual(SapHanaSchemaText.CreateSchemaSql, actual);
        }

        [TestMethod]
        public void TestSchemaExistsSqlIsTheCheckStatementOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaExistsSql;

            // Assert
            Assert.AreEqual(SapHanaSchemaText.SchemaExistsSql, actual);
        }

        [TestMethod]
        public void TestSchemaParameterPrefixIsThePrefixOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaParameterPrefix;

            // Assert
            Assert.AreEqual(":", actual);
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
