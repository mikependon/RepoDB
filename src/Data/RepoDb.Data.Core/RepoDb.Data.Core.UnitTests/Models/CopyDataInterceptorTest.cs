#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Data.Options;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyDataInterceptorTest
    {
        #region Classes

        private class PassThroughInterceptor : CopyDataInterceptor
        {
            public PassThroughInterceptor(CopyDataInterceptorOptions options)
                : base(options)
            {
            }

            public CopyDataInterceptorOptions GetOptions() =>
                Options;
        }

        private class UpperCaseInterceptor : CopyDataInterceptor
        {
            public UpperCaseInterceptor()
                : base(new CopyDataInterceptorOptions { CopyInterceptionLevel = CopyInterceptionLevel.RowAndTable })
            {
            }

            public override CopyDataRow Transform(CopyDataRow row) =>
                new CopyDataRow(row.Column, (row.Value as string)?.ToUpperInvariant() ?? row.Value);

            public override CopyDataTable Transform(CopyDataTable table) =>
                new CopyDataTable(table.DataRows.Select(Transform).ToList());
        }

        #endregion

        #region Constructor

        [TestMethod]
        public void TestCopyDataInterceptorConstructorSetsTheOptions()
        {
            // Setup
            var options = new CopyDataInterceptorOptions { RetryCount = 2 };

            // Act
            var actual = new PassThroughInterceptor(options);

            // Assert
            Assert.AreSame(options, actual.GetOptions());
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataInterceptorConstructorIfTheOptionsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new PassThroughInterceptor(null));
        }

        #endregion

        #region Transform

        [TestMethod]
        public void TestCopyDataInterceptorReturnsTheRowAsIs()
        {
            // Setup
            var interceptor = new PassThroughInterceptor(new CopyDataInterceptorOptions());
            var row = new CopyDataRow(new CopyDataColumn("Name", typeof(string)), "Michael");

            // Act
            var actual = interceptor.Transform(row);

            // Assert
            Assert.AreSame(row, actual);
        }

        [TestMethod]
        public void TestCopyDataInterceptorReturnsTheTableAsIs()
        {
            // Setup
            var interceptor = new PassThroughInterceptor(new CopyDataInterceptorOptions());
            var table = new CopyDataTable(new List<CopyDataRow>());

            // Act
            var actual = interceptor.Transform(table);

            // Assert
            Assert.AreSame(table, actual);
        }

        [TestMethod]
        public void TestCopyDataInterceptorCanOverrideTheTransformationOfARow()
        {
            // Setup
            var interceptor = new UpperCaseInterceptor();
            var row = new CopyDataRow(new CopyDataColumn("Name", typeof(string)), "michael");

            // Act
            var actual = interceptor.Transform(row);

            // Assert
            Assert.AreEqual("MICHAEL", actual.Value);
            Assert.AreSame(row.Column, actual.Column);
        }

        [TestMethod]
        public void TestCopyDataInterceptorCanOverrideTheTransformationOfATable()
        {
            // Setup
            var interceptor = new UpperCaseInterceptor();
            var column = new CopyDataColumn("Name", typeof(string));
            var table = new CopyDataTable(new List<CopyDataRow> { new CopyDataRow(column, "a"), new CopyDataRow(column, "b") });

            // Act
            var actual = interceptor.Transform(table);

            // Assert
            CollectionAssert.AreEqual(new object[] { "A", "B" }, actual.DataRows.Select(r => r.Value).ToArray());
        }

        #endregion

        #region Contracts

        [TestMethod]
        public void TestCopyDataInterceptorIsACopyDataInterceptorOfRowsAndTables()
        {
            // Setup
            var actual = new PassThroughInterceptor(new CopyDataInterceptorOptions());

            // Assert
            Assert.IsInstanceOfType<ICopyDataInterceptor>(actual);
            Assert.IsInstanceOfType<IDataInterceptor<CopyDataRow>>(actual);
            Assert.IsInstanceOfType<IDataInterceptor<CopyDataTable>>(actual);
        }

        #endregion
    }
}
