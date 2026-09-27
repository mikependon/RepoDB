#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Extensions;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        #region AsQuoted

        [TestMethod]
        public void TestAuroraDbQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = "`Field`".AsQuoted(setting);

            // Assert
            Assert.AreEqual("`Field`", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = "` Field `".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " ` Field ` ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("` Field `", result, StringComparer.Ordinal);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestAuroraDbQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " ` Field ` ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = "` Field `".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
