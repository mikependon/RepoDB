#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    [TestClass]
    public class MariaDbConnectorSchemaCreationSupportTest
    {
        #region Private Variables

        private readonly MariaDbConnectorSchemaComposer _composer = new MariaDbConnectorSchemaComposer();

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
            Assert.AreEqual(MariaDbConnectorSchemaText.CreateSchemaSql, actual);
        }

        [TestMethod]
        public void TestSchemaExistsSqlIsTheCheckStatementOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaExistsSql;

            // Assert
            Assert.AreEqual(MariaDbConnectorSchemaText.SchemaExistsSql, actual);
        }

        [TestMethod]
        public void TestSchemaParameterPrefixIsThePrefixOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaParameterPrefix;

            // Assert
            Assert.AreEqual("@", actual);
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestQuoteSchemaNameQuotesTheNameInTheDialectOfTheProvider()
        {
            // Act
            var actual = _composer.QuoteSchemaName("Sales");

            // Assert
            Assert.AreEqual("`Sales`", actual);
        }

        #endregion
    }
}
