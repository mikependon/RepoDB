using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QuotationTest
    {
        private IDbSetting m_dbSetting = new CustomDbSetting();

        #region AsQuoted

        [TestMethod]
        public void TestAsQuoted()
        {
            // Setup
            var text = "Value";

            //  
            var actual = text.AsQuoted(m_dbSetting);
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
            var actual = text.AsQuoted(m_dbSetting);
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
            var actual = text.AsQuoted(true, m_dbSetting);
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
            var actual = text.AsQuoted(true, m_dbSetting);
            var expected = "[Database].[Schema].[Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithDottedTable()
        {
            // Setup
            var text = "[Schema].[Dotted.Name]";

            // Act
            var actual = text.AsQuoted(true, m_dbSetting);
            var expected = "[Schema].[Dotted.Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithDottedTableWithDatabase()
        {
            // Setup
            var text = "[Database].[Schema].[Dotted.Name]";

            // Act
            var actual = text.AsQuoted(true, m_dbSetting);
            var expected = "[Database].[Schema].[Dotted.Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsQuotedWithDottedTableWithUnquotedDatabaseAndSchema()
        {
            // Setup
            var text = "Database.Schema.[Dotted.Name]";

            // Act
            var actual = text.AsQuoted(true, m_dbSetting);
            var expected = "[Database].[Schema].[Dotted.Name]";

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
            var actual = text.AsUnquoted(m_dbSetting);
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
            var actual = text.AsUnquoted(m_dbSetting);
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
            var actual = text.AsUnquoted(true, m_dbSetting);
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
            var actual = text.AsUnquoted(true, m_dbSetting);
            var expected = "Database.Schema.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithDottedTable()
        {
            // Setup
            var text = "[Schema].[Dotted.Name]";

            // Act
            var actual = text.AsUnquoted(true, m_dbSetting);
            var expected = "Schema.Dotted.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithDottedTableWithDatabase()
        {
            // Setup
            var text = "[Database].[Schema].[Dotted.Name]";

            // Act
            var actual = text.AsUnquoted(true, m_dbSetting);
            var expected = "Database.Schema.Dotted.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAsUnquotedWithDottedTableWithUnquotedDatabaseAndSchema()
        {
            // Setup
            var text = "Database.Schema.[Dotted.Name]";

            // Act
            var actual = text.AsUnquoted(true, m_dbSetting);
            var expected = "Database.Schema.Dotted.Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
