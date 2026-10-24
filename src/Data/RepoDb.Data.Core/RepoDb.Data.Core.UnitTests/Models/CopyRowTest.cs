#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyRowTest
    {
        #region Helpers

        private static CopyRow Create() =>
            new CopyRow(
                new[] { new CopyColumn("Id", typeof(int)), new CopyColumn("Name", typeof(string)) },
                new object[] { 1, "Michael" });

        #endregion

        #region Constructors

        [TestMethod]
        public void TestCopyRowConstructorSetsTheColumnsAndTheValues()
        {
            // Act
            var actual = Create();

            // Assert
            Assert.AreEqual(2, actual.Count);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, new List<string> { actual.Columns[0].Name, actual.Columns[1].Name });
            CollectionAssert.AreEqual(new object[] { 1, "Michael" }, new List<object>(actual.Values));
        }

        [TestMethod]
        public void TestCopyRowConstructorAcceptsARowWithoutColumns()
        {
            // Act
            var actual = new CopyRow(new CopyColumn[0], new object[0]);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowConstructorIfTheColumnsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CopyRow((IEnumerable<CopyColumn>)null, new object[0]));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowConstructorIfTheValuesAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CopyRow(new CopyColumn[0], null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowConstructorIfAColumnIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CopyRow(new CopyColumn[] { null }, new object[] { 1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowConstructorIfTheNumberOfValuesIsNotTheNumberOfColumns()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CopyRow(new[] { new CopyColumn("Id", typeof(int)) }, new object[] { 1, 2 }));
        }

        #endregion

        #region Indexers

        [TestMethod]
        public void TestCopyRowGetsAndSetsTheValueByPosition()
        {
            // Setup
            var row = Create();

            // Act
            row[1] = "Pendon";

            // Assert
            Assert.AreEqual(1, row[0]);
            Assert.AreEqual("Pendon", row[1]);
        }

        [TestMethod]
        public void TestCopyRowGetsAndSetsTheValueByNameWithoutTheCase()
        {
            // Setup
            var row = Create();

            // Act
            row["NAME"] = "Pendon";

            // Assert
            Assert.AreEqual("Pendon", row["name"]);
            Assert.AreEqual("Pendon", row[1]);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowIfTheNameOfTheColumnDoesNotExist()
        {
            // Setup
            var row = Create();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => row["Missing"].ToString());
            Assert.Throws<ArgumentException>(() => row["Missing"] = 1);
        }

        [TestMethod]
        public void TestCopyRowIndexOfAndContains()
        {
            // Setup
            var row = Create();

            // Act/Assert
            Assert.AreEqual(1, row.IndexOf("name"));
            Assert.AreEqual(-1, row.IndexOf("Missing"));
            Assert.IsTrue(row.Contains("ID"));
            Assert.IsFalse(row.Contains("Missing"));
        }

        #endregion

        #region Add And Remove

        [TestMethod]
        public void TestCopyRowAddAppendsTheColumnAndItsValue()
        {
            // Setup
            var row = Create();

            // Act
            var actual = row.Add("Birthday", typeof(DateTime), new DateTime(2000, 1, 2));

            // Assert
            Assert.AreSame(row, actual);
            Assert.AreEqual(3, row.Count);
            Assert.AreEqual("Birthday", row.Columns[2].Name);
            Assert.AreEqual(typeof(DateTime), row.Columns[2].Type);
            Assert.AreEqual(new DateTime(2000, 1, 2), row["Birthday"]);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowAddIfTheRowHasTheColumn()
        {
            // Setup
            var row = Create();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => row.Add(new CopyColumn("NAME", typeof(string)), "x"));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowAddIfTheColumnIsNull()
        {
            // Setup
            var row = Create();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => row.Add(null, 1));
        }

        [TestMethod]
        public void TestCopyRowRemoveTakesTheColumnAndItsValueOut()
        {
            // Setup
            var row = Create();

            // Act
            var actual = row.Remove("id");

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(1, row.Count);
            Assert.AreEqual("Name", row.Columns[0].Name);
            Assert.AreEqual("Michael", row[0]);
        }

        [TestMethod]
        public void TestCopyRowRemoveOfAColumnThatTheRowDoesNotHaveReturnsFalse()
        {
            // Setup
            var row = Create();

            // Act
            var actual = row.Remove("Missing");

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(2, row.Count);
        }

        [TestMethod]
        public void TestCopyRowThatSharesTheColumnsDoesNotChangeTheOtherRowsWhenItAddsOrRemovesAColumn()
        {
            // Setup
            var columns = new List<CopyColumn> { new CopyColumn("Id", typeof(int)), new CopyColumn("Name", typeof(string)) };
            var first = new CopyRow(columns, new object[] { 1, "a" });
            var second = new CopyRow(columns, new object[] { 2, "b" });

            // Act
            first.Add("Extra", typeof(int), 9);
            second.Remove("Name");

            // Assert
            Assert.AreEqual(3, first.Count);
            Assert.AreEqual(1, second.Count);
            Assert.AreEqual(2, columns.Count);
        }

        #endregion
    }
}
