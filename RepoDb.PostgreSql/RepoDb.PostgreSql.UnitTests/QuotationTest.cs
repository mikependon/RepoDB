using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
        }

        #region AsQuoted

        [TestMethod]
        public void TestPostgreSqlQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = "\"Field\"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\"Field\"", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = "\" Field \"".AsQuoted(setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " \" Field \" ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("\" Field \"", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestPostgreSqlQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " \" Field \" ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = "\" Field \"".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestPostgreSqlQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion
    }
}
