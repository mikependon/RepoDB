#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Helpers;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class RowInserterTest
    {
        #region Helpers

        private static IList<object> Rows(int count) =>
            Enumerable.Range(1, count)
                .Select(i => (object)new Dictionary<string, object>
                {
                    ["Id"] = i,
                    ["Name"] = "Name " + i
                })
                .ToList();

        #endregion

        #region Insert

        [TestMethod]
        public void TestInsertInsertsEveryRowAndReturnsTheNumberOfInsertedRows()
        {
            // Setup
            var connection = new FakeDbConnection();

            // Act
            var actual = RowInserter.Insert(connection, "Customer", Rows(3), null, null, null, null);

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual(3, connection.Executions.Count(e => e.Kind == "NonQuery"));
        }

        [TestMethod]
        public void TestInsertOfNoRowInsertsNothing()
        {
            // Setup
            var connection = new FakeDbConnection();

            // Act
            var actual = RowInserter.Insert(connection, "Customer", new List<object>(), null, null, null, null);

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.Executions.Count);
        }

        [TestMethod]
        public void TestInsertKeepsThePrefixOfTheNamesOfTheParametersThatItIsNotTheDollar()
        {
            // Setup
            var connection = new FakeDbConnection();

            // Act
            RowInserter.Insert(connection, "Customer", Rows(1), null, null, null, null);

            // Assert
            var execution = connection.Executions.Single(e => e.Kind == "NonQuery");
            Assert.IsTrue(execution.Parameters.Keys.All(k => k.StartsWith("@p", System.StringComparison.Ordinal)));
            StringAssert.Contains(execution.CommandText, "@p0", System.StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestInsertSetsTheNamesOfTheParametersWithoutTheDollarPrefix()
        {
            // Setup
            var connection = new FakeDollarDbConnection();

            // Act
            RowInserter.Insert(connection, "Customer", Rows(2), null, null, null, null);

            // Assert
            var executions = connection.Executions.Where(e => e.Kind == "NonQuery").ToList();
            Assert.AreEqual(2, executions.Count);
            Assert.IsTrue(executions.All(e => e.Parameters.Keys.All(k => k.StartsWith("p", System.StringComparison.Ordinal))));
            Assert.IsTrue(executions.All(e => e.CommandText.Contains("$p0")));
            Assert.IsTrue(executions.All(e => !e.CommandText.Contains("$$")));
        }

        [TestMethod]
        public async Task TestInsertAsyncInsertsEveryRowAndReturnsTheNumberOfInsertedRows()
        {
            // Setup
            var connection = new FakeDbConnection();

            // Act
            var actual = await RowInserter.InsertAsync(connection, "Customer", Rows(3), null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public async Task TestInsertAsyncSetsTheNamesOfTheParametersWithoutTheDollarPrefix()
        {
            // Setup
            var connection = new FakeDollarDbConnection();

            // Act
            await RowInserter.InsertAsync(connection, "Customer", Rows(1), null, null, null, null, CancellationToken.None);

            // Assert
            var execution = connection.Executions.Single(e => e.Kind == "NonQuery");
            Assert.IsTrue(execution.Parameters.Keys.All(k => k.StartsWith("p", System.StringComparison.Ordinal)));
        }

        #endregion
    }
}
