#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.Extensions;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        #region AsQuoted

        [TestMethod]
        public void TestEnterpriseDbQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = "\" Field \"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " \" Field \" ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestEnterpriseDbQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " \" Field \" ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = "\" Field \"".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestEnterpriseDbQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
