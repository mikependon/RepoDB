#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Extensions;

namespace RepoDb.MySql.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMySql();
        }

        #region AsQuoted

        [TestMethod]
        public void TestMySqlQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = "`Field`".AsQuoted(setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = "` Field `".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " ` Field ` ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestMySqlQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " ` Field ` ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = "` Field `".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
