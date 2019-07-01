using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        #region AsAlphaNumeric
        
        [TestMethod]
        public void TestAsAlphaNumericAsciiNonAlpha()
        {
            // Setup
            string text = "!@#$%^&*abcd(){}|[]";

            // Act
            var actual = text.AsAlphaNumeric(false);
            var expected = "________abcd_______";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsAlphaNumericNonAsciiRtl()
        {
            // Setup
            string text = "foobarښڛڜڝڞڟڠڡ1 2 3";

            // Act
            var actual = text.AsAlphaNumeric(false);
            var expected = "foobar________1_2_3";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsAlphaNumericNonAsciiWithTrim()
        {
            // Setup
            string text = "    ӨөӪӫӬӭӮӯfoobarȻȼȽȾȿɀɁ1 2 3   ";

            // Act
            var actual = text.AsAlphaNumeric(true);
            var expected = "________foobar_______1_2_3";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsAlphaNumericUnicodeLongCodePoint()
        {
            //Note: 0x10FFFC is a single Unicode code point, but is treated as 2 Unicode code units (and 2 chars).

            // Setup
            string text = char.ConvertFromUtf32(0x10FFFC)
                        + "0123456789";

            // Act
            var actual = text.AsAlphaNumeric(true);
            var expected = "__0123456789";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region AsQuoted

        [TestMethod]
        public void TestAsQuoted()
        {
            // Setup
            var text = "Value";

            // Act
            var actual = text.AsQuoted();
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
            var actual = text.AsQuoted();
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
            var actual = text.AsQuoted(true);
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
            var actual = text.AsQuoted(true);
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
            var actual = text.AsUnquoted();
            var expected = "Value";

            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestAsUnquotedWithQuotes()
        {
            // Setup
            var text = @"'Database'.""Table""";

            // Act
            var actual = text.AsUnquoted();
            var expected = "Database.Table";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithSpaces()
        {
            // Setup
            var text = " [Field Value] ";

            // Act
            var actual = text.AsUnquoted();
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
            var actual = text.AsUnquoted(true);
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
            var actual = text.AsUnquoted(true);
            var expected = "Database.Schema.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestAsUnquotedWithSeparatorNotFound()
        {
            // Setup
            var text = "[Database]";

            // Act
            var actual = text.AsUnquoted(true, ".");
            var expected = "Database";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithWhitespaceInput()
        {
            // Setup
            var text = "   ";

            // Act
            var actual = text.AsUnquoted(true, ".");
            var expected = "";

            // Assert
            Assert.AreEqual(expected, actual);
        }


        #endregion
    }
}
