#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;

namespace RepoDb.SapHana.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        #region AsQuoted

        [TestMethod]
        public void TestSapHanaQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = "\" Field \"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " \" Field \" ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestSapHanaQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " \" Field \" ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = "\" Field \"".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestSapHanaQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
