#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Attributes.Parameter.DuckDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.DuckDb.UnitTests.Attributes.Parameter.DuckDb
{
    [TestClass]
    public class SourceVersionAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        #region Classes

        private class SourceVersionAttributeTestClass
        {
            [SourceVersion(DataRowVersion.Original)]
            public object ColumnName { get; set; }
        }

        #endregion

        // DuckDBParameter.SourceVersion's setter does not persist the assigned value (confirmed empirically -
        // see the remarks on SourceVersionAttribute), so these tests only verify that attribute-driven
        // assignment runs without throwing and still produces the expected parameter, rather than asserting
        // a round-tripped value the driver itself does not support.

        [TestMethod]
        public void TestSourceVersionAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new DuckDBConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceVersionAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);
            Assert.IsNotNull(command.Parameters["ColumnName"]);
        }

        [TestMethod]
        public void TestSourceVersionAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DuckDBConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SourceVersionAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);
            Assert.IsNotNull(command.Parameters["ColumnName"]);
        }
    }
}
