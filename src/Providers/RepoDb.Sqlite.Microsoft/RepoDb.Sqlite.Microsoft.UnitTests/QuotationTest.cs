#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;

namespace RepoDb.Sqlite.Microsoft.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSqlite();
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
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[Field]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " [ Field ] ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
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
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion

        #endregion
    }
}
