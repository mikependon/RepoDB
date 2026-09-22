#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Extensions;

namespace RepoDb.DuckDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        #region AsQuoted

        [TestMethod]
        public void TestDuckDbQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = "\" Field \"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " \" Field \" ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\" Field \"", result, StringComparer.Ordinal);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestDuckDbQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " \" Field \" ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = "\" Field \"".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
