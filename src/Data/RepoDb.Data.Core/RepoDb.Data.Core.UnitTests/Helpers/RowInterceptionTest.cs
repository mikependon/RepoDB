#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Helpers;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class RowInterceptionTest
    {
        #region Helpers

        private static CopyRow Row() =>
            new CopyRow(new[] { new CopyColumn("Value", typeof(string)) }, new object[] { "x" });

        #endregion

        #region Normalize

        [TestMethod]
        public void TestNormalizeGivesNullIfThereAreNoInterceptors()
        {
            // Act/Assert
            Assert.IsNull(RowInterception.Normalize(null));
            Assert.IsNull(RowInterception.Normalize(new List<IDataInterceptor<CopyRow>>()));
        }

        [TestMethod]
        public void TestNormalizeGivesTheInterceptors()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<CopyRow>> { new FakeDataInterceptor(row => row) };

            // Act
            var actual = RowInterception.Normalize(interceptors);

            // Assert
            Assert.AreSame(interceptors, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnNormalizeIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => RowInterception.Normalize(new List<IDataInterceptor<CopyRow>> { null }));
        }

        #endregion

        #region Read

        [TestMethod]
        public void TestReadGivesTheColumnsAndTheValuesOfTheCurrentRowWithTheDatabaseNullsAsNull()
        {
            // Setup
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(1, DBNull.Value);
            using (var reader = table.CreateDataReader())
            {
                reader.Read();
                var columns = RowInterception.GetColumns(reader);

                // Act
                var actual = RowInterception.Read(reader, columns);

                // Assert
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(typeof(int), actual.Columns[0].Type);
                Assert.AreEqual(1, actual["Id"]);
                Assert.IsNull(actual["Name"]);
            }
        }

        #endregion

        #region Apply

        [TestMethod]
        public void TestApplyCallsTheInterceptorsInOrderAndGivesEachOneTheResultOfThePreviousOne()
        {
            // Setup
            var order = new List<string>();
            var interceptors = new List<IDataInterceptor<CopyRow>>
            {
                new FakeDataInterceptor(row => { order.Add("first:" + row["Value"]); row["Value"] = row["Value"] + "1"; return row; }),
                new FakeDataInterceptor(row => { order.Add("second:" + row["Value"]); return new CopyRow(row.Columns, new object[] { row["Value"] + "2" }); }),
                new FakeDataInterceptor(row => { order.Add("third:" + row["Value"]); row["Value"] = row["Value"] + "3"; return row; })
            };

            // Act
            var actual = RowInterception.Apply(Row(), interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "first:x", "second:x1", "third:x12" }, order);
            Assert.AreEqual("x123", actual["Value"]);
        }

        [TestMethod]
        public void ThrowExceptionOnApplyIfAnInterceptorReturnsNull()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<CopyRow>> { new FakeDataInterceptor(row => row), new FakeDataInterceptor(row => null) };

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => RowInterception.Apply(Row(), interceptors));
        }

        #endregion
    }
}
