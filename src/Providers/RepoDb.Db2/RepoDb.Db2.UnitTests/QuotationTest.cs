#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        #region AsQuoted

        [TestMethod]
        public void TestDb2QuotationForQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "Field".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestDb2QuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestDb2QuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestDb2QuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestDb2QuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "\" Field \"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestDb2QuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " \" Field \" ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestDb2QuotationForUnquoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "\"Field\"".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestDb2QuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " \" Field \" ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestDb2QuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "\" Field \"".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestDb2QuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestDb2QuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestDb2QuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
