using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDb;
using RepoDb.Extensions;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        #region AsQuoted

        [TestMethod]
        public void TestMariaDbQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("`Field`", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = "`Field`".AsQuoted(setting);

            // Assert
            Assert.AreEqual("`Field`", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = "` Field `".AsQuoted(setting);

            // Assert
            Assert.AreEqual("` Field `", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " ` Field ` ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("` Field `", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestMariaDbQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " ` Field ` ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = "` Field `".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMariaDbQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
