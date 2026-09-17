#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Extensions;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        #region AsQuoted

        [TestMethod]
        public void TestClickHouseQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = "`Field`".AsQuoted(setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = "` Field `".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " ` Field ` ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestClickHouseQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " ` Field ` ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = "` Field `".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
