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
using IBM.Data.Db2;

namespace RepoDb.Schema.Db2.UnitTests
{
    [TestClass]
    public class Db2CreateSchemaTest
    {
        #region Statements

        [TestMethod]
        public void TestDb2CreateSchemaStatementQuotesTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(Db2SchemaText.CreateSchemaSql, Db2SchemaHelper.Quote("Sales"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"Sales\"", actual);
        }

        [TestMethod]
        public void TestDb2CreateSchemaStatementQuotesANameWithSpaces()
        {
            // Act
            var actual = string.Format(Db2SchemaText.CreateSchemaSql, Db2SchemaHelper.Quote("Sales Order"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"Sales Order\"", actual);
        }

        [TestMethod]
        public void TestDb2CreateSchemaStatementEscapesTheQuoteInTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(Db2SchemaText.CreateSchemaSql, Db2SchemaHelper.Quote("A\"B; DROP TABLE X"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"A\"\"B; DROP TABLE X\"", actual);
        }

        [TestMethod]
        public void TestDb2SchemaExistsStatementCountsTheSchemasThatHaveTheNameByAParameter()
        {
            // Act
            var actual = string.Format(Db2SchemaText.SchemaExistsSql, ":SchemaName");

            // Assert
            Assert.AreEqual("SELECT COUNT(*) FROM SYSCAT.SCHEMATA WHERE SCHEMANAME = :SchemaName", actual);
        }

        #endregion

        #region Signature

        [TestMethod]
        public void TestDb2CreateSchemaHasTheArgumentsOfTheOperationAndTheirDefaults()
        {
            // Act
            var method = typeof(CreateSchemaExtension).GetMethod("CreateSchema");
            var parameters = method.GetParameters();

            // Assert
            CollectionAssert.AreEqual(new[] { "connection", "schemaName", "commandTimeout", "traceKey", "trace", "transaction" }, parameters.Select(parameter => parameter.Name).ToArray());
            Assert.AreEqual(typeof(DB2Connection), parameters[0].ParameterType);
            Assert.AreEqual(typeof(bool), method.ReturnType);
            Assert.IsNull(parameters[2].DefaultValue);
            Assert.AreEqual("CreateSchema", parameters[3].DefaultValue);
            Assert.IsNull(parameters[4].DefaultValue);
            Assert.IsNull(parameters[5].DefaultValue);
        }

        [TestMethod]
        public void TestDb2CreateSchemaAsyncHasTheArgumentsOfTheOperationAndTheCancellationToken()
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
        public void TestDb2CreateSchemaIsAnExtensionMethodOfTheConnection()
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
        public void ThrowExceptionOnDb2CreateSchemaIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((DB2Connection)null).CreateSchema("Sales"));
        }

        [TestMethod]
        public async Task ThrowExceptionOnDb2CreateSchemaAsyncIfTheConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((DB2Connection)null).CreateSchemaAsync("Sales"));
        }

        #endregion
    }
}
