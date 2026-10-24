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
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests
{
    [TestClass]
    public class AuroraDbPostgreSqlCreateSchemaTest
    {
        #region Statements

        [TestMethod]
        public void TestAuroraDbPostgreSqlCreateSchemaStatementQuotesTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(AuroraDbPostgreSqlSchemaText.CreateSchemaSql, AuroraDbPostgreSqlSchemaHelper.Quote("Sales"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"Sales\"", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCreateSchemaStatementQuotesANameWithSpaces()
        {
            // Act
            var actual = string.Format(AuroraDbPostgreSqlSchemaText.CreateSchemaSql, AuroraDbPostgreSqlSchemaHelper.Quote("Sales Order"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"Sales Order\"", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCreateSchemaStatementEscapesTheQuoteInTheNameOfTheSchema()
        {
            // Act
            var actual = string.Format(AuroraDbPostgreSqlSchemaText.CreateSchemaSql, AuroraDbPostgreSqlSchemaHelper.Quote("A\"B; DROP TABLE X"));

            // Assert
            Assert.AreEqual("CREATE SCHEMA \"A\"\"B; DROP TABLE X\"", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaExistsStatementCountsTheSchemasThatHaveTheNameByAParameter()
        {
            // Act
            var actual = string.Format(AuroraDbPostgreSqlSchemaText.SchemaExistsSql, "@SchemaName");

            // Assert
            Assert.AreEqual("SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = @SchemaName", actual);
        }

        #endregion

        #region Signature

        [TestMethod]
        public void TestAuroraDbPostgreSqlCreateSchemaHasTheArgumentsOfTheOperationAndTheirDefaults()
        {
            // Act
            var method = typeof(CreateSchemaExtension).GetMethod("CreateSchema");
            var parameters = method.GetParameters();

            // Assert
            CollectionAssert.AreEqual(new[] { "connection", "schemaName", "commandTimeout", "traceKey", "trace", "transaction" }, parameters.Select(parameter => parameter.Name).ToArray());
            Assert.AreEqual(typeof(AuroraDbConnection), parameters[0].ParameterType);
            Assert.AreEqual(typeof(bool), method.ReturnType);
            Assert.IsNull(parameters[2].DefaultValue);
            Assert.AreEqual("CreateSchema", parameters[3].DefaultValue);
            Assert.IsNull(parameters[4].DefaultValue);
            Assert.IsNull(parameters[5].DefaultValue);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCreateSchemaAsyncHasTheArgumentsOfTheOperationAndTheCancellationToken()
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
        public void TestAuroraDbPostgreSqlCreateSchemaIsAnExtensionMethodOfTheConnection()
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
        public void ThrowExceptionOnAuroraDbPostgreSqlCreateSchemaIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((AuroraDbConnection)null).CreateSchema("Sales"));
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbPostgreSqlCreateSchemaAsyncIfTheConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((AuroraDbConnection)null).CreateSchemaAsync("Sales"));
        }

        #endregion
    }
}
