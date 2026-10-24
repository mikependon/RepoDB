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
using Microsoft.Data.SqlClient;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    [TestClass]
    public class SqlServerCreateSchemaTest
    {
        #region Statements

        [TestMethod]
        public void TestSqlServerCreateSchemaStatementQuotesTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(SqlServerSchemaText.CreateSchemaSql, SqlServerSchemaHelper.Quote("Sales"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA [Sales]", actual);
        }

        [TestMethod]
        public void TestSqlServerCreateSchemaStatementQuotesANameWithSpaces()
        {
            // Act
            var actual = string.Format(SqlServerSchemaText.CreateSchemaSql, SqlServerSchemaHelper.Quote("Sales Order"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA [Sales Order]", actual);
        }

        [TestMethod]
        public void TestSqlServerCreateSchemaStatementEscapesTheQuoteInTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(SqlServerSchemaText.CreateSchemaSql, SqlServerSchemaHelper.Quote("A]B; DROP TABLE X"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA [A]]B; DROP TABLE X]", actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaExistsStatementCountsTheSchemasThatHaveTheNameByAParameter()
        {
            // Act
            var actual = string.Format(SqlServerSchemaText.SchemaExistsSql, "@SchemaName");

            // Assert
            Assert.AreEqual("SELECT COUNT(*) FROM sys.schemas WHERE name = @SchemaName", actual);
        }

        #endregion

        #region Signature

        [TestMethod]
        public void TestSqlServerCreateSchemaHasTheArgumentsOfTheOperationAndTheirDefaults()
        {
            // Act
            var method = typeof(CreateSchemaExtension).GetMethod("CreateSchema");
            var parameters = method.GetParameters();

            // Assert
            CollectionAssert.AreEqual(new[] { "connection", "schemaName", "commandTimeout", "traceKey", "trace", "transaction" }, parameters.Select(parameter => parameter.Name).ToArray());
            Assert.AreEqual(typeof(SqlConnection), parameters[0].ParameterType);
            Assert.AreEqual(typeof(bool), method.ReturnType);
            Assert.IsNull(parameters[2].DefaultValue);
            Assert.AreEqual("CreateSchema", parameters[3].DefaultValue);
            Assert.IsNull(parameters[4].DefaultValue);
            Assert.IsNull(parameters[5].DefaultValue);
        }

        [TestMethod]
        public void TestSqlServerCreateSchemaAsyncHasTheArgumentsOfTheOperationAndTheCancellationToken()
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
        public void TestSqlServerCreateSchemaIsAnExtensionMethodOfTheConnection()
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
        public void ThrowExceptionOnSqlServerCreateSchemaIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((SqlConnection)null).CreateSchema("Sales"));
        }

        [TestMethod]
        public async Task ThrowExceptionOnSqlServerCreateSchemaAsyncIfTheConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((SqlConnection)null).CreateSchemaAsync("Sales"));
        }

        #endregion
    }
}
