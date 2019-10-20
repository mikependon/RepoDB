using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        #region AsQuoted

        [TestMethod]
        public void TestAsQuoted()
        {
            // Setup
            var text = "Value";

            //  
            var actual = text.AsQuoted(Helper.DbSetting);
            var expected = "[Value]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithSpaces()
        {
            // Setup
            var text = " Field Value ";

            // Act
            var actual = text.AsQuoted(Helper.DbSetting);
            var expected = "[ Field Value ]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithSpacesAsTrimmed()
        {
            // Setup
            var text = " Field Value ";

            // Act
            var actual = text.AsQuoted(true, Helper.DbSetting);
            var expected = "[Field Value]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithSeparators()
        {
            // Setup
            var text = "Database.Schema.Name";

            // Act
            var actual = text.AsQuoted(true, Helper.DbSetting);
            var expected = "[Database].[Schema].[Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region AsUnQuoted

        [TestMethod]
        public void TestAsUnquoted()
        {
            // Setup
            var text = "[Value]";

            // Act
            var actual = text.AsUnquoted(Helper.DbSetting);
            var expected = "Value";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithSpaces()
        {
            // Setup
            var text = " [Field Value] ";

            // Act
            var actual = text.AsUnquoted(Helper.DbSetting);
            var expected = " Field Value ";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithSpacesAsTrimmed()
        {
            // Setup
            var text = " [Field Value] ";

            // Act
            var actual = text.AsUnquoted(true, Helper.DbSetting);
            var expected = "Field Value";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithSeparators()
        {
            // Setup
            var text = "[Database].[Schema].[Name]";

            // Act
            var actual = text.AsUnquoted(true, Helper.DbSetting);
            var expected = "Database.Schema.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
