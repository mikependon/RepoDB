using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        #region AsQuoted

        [TestMethod]
        public void TestOracleQuotationForQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Act
            var result = "Field".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestOracleQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestOracleQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestOracleQuotationForUnquoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Act
            var result = "\"Field\"".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestOracleQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
