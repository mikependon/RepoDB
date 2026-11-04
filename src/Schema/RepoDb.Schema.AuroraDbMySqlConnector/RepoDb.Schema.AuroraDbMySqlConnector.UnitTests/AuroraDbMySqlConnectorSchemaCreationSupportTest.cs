#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaCreationSupportTest
    {
        #region Private Variables

        private readonly AuroraDbMySqlConnectorSchemaComposer _composer = new AuroraDbMySqlConnectorSchemaComposer();

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
            Assert.AreEqual(AuroraDbMySqlConnectorSchemaText.CreateSchemaSql, actual);
        }

        [TestMethod]
        public void TestSchemaExistsSqlIsTheCheckStatementOfTheProvider()
        {
            // Act
            var actual = _composer.SchemaExistsSql;

            // Assert
            Assert.AreEqual(AuroraDbMySqlConnectorSchemaText.SchemaExistsSql, actual);
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
