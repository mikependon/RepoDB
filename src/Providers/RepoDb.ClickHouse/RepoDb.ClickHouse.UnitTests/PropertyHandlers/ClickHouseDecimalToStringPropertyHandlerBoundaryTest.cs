using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.ClickHouse;
using System;

namespace RepoDb.ClickHouse.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class ClickHouseDecimalToStringPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Treats absent and blank numeric text as null.
        /// </summary>
        /// <param name="value">The blank input.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [TestMethod]
        public void SetBlankTextReturnsNull(string value)
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();


            // Act
            var actual = handler.Set(value,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves database null values.
        /// </summary>
        [TestMethod]
        public void GetDbNullReturnsNull()
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();


            // Act
            var actual = handler.Get(DBNull.Value,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Uses a decimal point in numeric text.
        /// </summary>
        [TestMethod]
        public void GetDecimalFormatsInvariantText()
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();


            // Act
            var actual = handler.Get(12.25m,
                null);


            // Assert
            Assert.AreEqual("12.25",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats convertible numeric input.
        /// </summary>
        [TestMethod]
        public void GetIntegerFormatsInvariantText()
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();


            // Act
            var actual = handler.Get(42,
                null);


            // Assert
            Assert.AreEqual("42",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves digits beyond the decimal range.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesWideValue()
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();
            const string expected = "99999999999999999999999999999999999999";
            var stored = handler.Set("  " + expected + "  ",
                null);


            // Act
            var actual = handler.Get(stored,
                null);


            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects malformed numeric text.
        /// </summary>
        [TestMethod]
        public void SetInvalidTextThrows()
        {
            // Setup
            var handler = new ClickHouseDecimalToStringPropertyHandler();


            // Act
            Action action = () => handler.Set("invalid",
                null);


            // Assert
            Assert.ThrowsExactly<FormatException>(action);
        }

        #endregion
    }
}