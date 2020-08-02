using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Data.SQLite;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        #region SDS

        #region AsQuoted

        [TestMethod]
        public void TestSqLiteQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = "[Field]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = "[ Field ]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " [ Field ] ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestSqLiteQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " [ Field ] ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = "[ Field ]".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestSqLiteQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion

        #endregion

        #region MDS

        #region AsQuoted

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuoted()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[Field]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[Field]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsQuoted(setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForQuotedForPreQuotedWithSpaceAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " [ Field ] ".AsQuoted(true, setting);

            // Assert
            Assert.AreEqual("[ Field ]", result);
        }

        #endregion

        #region AsUnquoted

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedAndTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " [ Field ] ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedNonTrimmed()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "[ Field ]".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlain()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = "Field".AsUnquoted(setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(setting);

            // Assert
            Assert.AreEqual(" Field ", result);
        }

        [TestMethod]
        public void TestMdsSqLiteQuotationForUnquotedAndTrimmedForPlainWithSpace()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Act
            var result = " Field ".AsUnquoted(true, setting);

            // Assert
            Assert.AreEqual("Field", result);
        }

        #endregion

        #endregion
    }
}
