#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;

namespace RepoDb.Turso.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseTurso();
        }

        #region MDS

        #region AsQuoted

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[Field]", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[Field]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[Field]", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " [ Field ] ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[ Field ]", result, StringComparer.Ordinal);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " [ Field ] ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        #endregion

        #endregion
    }
}
