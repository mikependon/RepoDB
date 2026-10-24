#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;

namespace RepoDb.Schema.MySqlConnector.UnitTests
{
    [TestClass]
    public class MySqlConnectorCreateSchemaTest
    {
        #region Statements

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaStatementQuotesTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(MySqlConnectorSchemaText.CreateSchemaSql, MySqlConnectorSchemaHelper.Quote("Sales"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA `Sales`", actual);
        }

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaStatementQuotesANameWithSpaces()
        {
            // Act
            var actual = string.Format(MySqlConnectorSchemaText.CreateSchemaSql, MySqlConnectorSchemaHelper.Quote("Sales Order"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA `Sales Order`", actual);
        }

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaStatementEscapesTheQuoteInTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(MySqlConnectorSchemaText.CreateSchemaSql, MySqlConnectorSchemaHelper.Quote("A`B; DROP TABLE X"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA `A``B; DROP TABLE X`", actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaExistsStatementCountsTheSchemasThatHaveTheNameByAParameter()
        {
            // Act
            var actual = string.Format(MySqlConnectorSchemaText.SchemaExistsSql, "@SchemaName");

            // Assert
            Assert.AreEqual("SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @SchemaName", actual);
        }

        #endregion

        #region Signature

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaHasTheArgumentsOfTheOperationAndTheirDefaults()
        {
            // Act
            var method = typeof(CreateSchemaExtension).GetMethod("CreateSchema");
            var parameters = method.GetParameters();

            // Assert
            CollectionAssert.AreEqual(new[] { "connection", "schemaName", "commandTimeout", "traceKey", "trace", "transaction" }, parameters.Select(parameter => parameter.Name).ToArray());
            Assert.AreEqual(typeof(MySqlConnection), parameters[0].ParameterType);
            Assert.AreEqual(typeof(bool), method.ReturnType);
            Assert.IsNull(parameters[2].DefaultValue);
            Assert.AreEqual("CreateSchema", parameters[3].DefaultValue);
            Assert.IsNull(parameters[4].DefaultValue);
            Assert.IsNull(parameters[5].DefaultValue);
        }

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaAsyncHasTheArgumentsOfTheOperationAndTheCancellationToken()
        {
            // Act
            var method = typeof(CreateSchemaExtension).GetMethod("CreateSchemaAsync");
            var parameters = method.GetParameters();

            // Assert
            CollectionAssert.AreEqual(new[] { "connection", "schemaName", "commandTimeout", "traceKey", "trace", "transaction", "cancellationToken" }, parameters.Select(parameter => parameter.Name).ToArray());
            Assert.AreEqual(typeof(Task<bool>), method.ReturnType);
            Assert.AreEqual("CreateSchema", parameters[3].DefaultValue);
        }

        [TestMethod]
        public void TestMySqlConnectorCreateSchemaIsAnExtensionMethodOfTheConnection()
        {
            // Act
            var methods = typeof(CreateSchemaExtension).GetMethods(BindingFlags.Public | BindingFlags.Static);

            // Assert
            Assert.AreEqual(2, methods.Length);
            Assert.IsTrue(methods.All(method => method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)));
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorCreateSchemaIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((MySqlConnection)null).CreateSchema("Sales"));
        }

        [TestMethod]
        public async Task ThrowExceptionOnMySqlConnectorCreateSchemaAsyncIfTheConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((MySqlConnection)null).CreateSchemaAsync("Sales"));
        }

        #endregion
    }
}
