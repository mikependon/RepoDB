using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.ClickHouse;
using System;

namespace RepoDb.ClickHouse.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class ClickHouseDecimalToNullableDecimalPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Handles an absent database decimal.
        /// </summary>
        [TestMethod]
        public void GetNullReturnsDefault()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

            // Act
            var actual = handler.Get(null,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Handles a database null decimal.
        /// </summary>
        [TestMethod]
        public void GetDbNullReturnsDefault()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves a native decimal value.
        /// </summary>
        [TestMethod]
        public void GetDecimalPreservesPrecision()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            const decimal expected = -123.456789m;

            // Act
            var actual = handler.Get(expected,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Converts invariant numeric text.
        /// </summary>
        [TestMethod]
        public void GetNumericTextUsesInvariantFormat()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

            // Act
            var actual = handler.Get("123.25",
                null);

            // Assert
            Assert.AreEqual(123.25m,
                actual);
        }

        /// <summary>
        /// Rejects malformed decimal input.
        /// </summary>
        [TestMethod]
        public void GetInvalidTextThrows()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

            // Act
            Action action = () => handler.Get("invalid",
                null);

            // Assert
            Assert.ThrowsExactly<FormatException>(action);
        }

        /// <summary>
        /// Round-trips the full decimal range through the driver type.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesDecimalBoundariesFor0()
        {
            // Setup
            string value = "0";
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            var expected = decimal.Parse(value,
                System.Globalization.CultureInfo.InvariantCulture);
            var stored = handler.Set(expected,
                null);

            // Act
            var actual = handler.Get(stored,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Round-trips the full decimal range through the driver type.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesDecimalBoundariesForMaximumDecimalValue()
        {
            // Setup
            string value = "79228162514264337593543950335";
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            var expected = decimal.Parse(value,
                System.Globalization.CultureInfo.InvariantCulture);
            var stored = handler.Set(expected,
                null);

            // Act
            var actual = handler.Get(stored,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Round-trips the full decimal range through the driver type.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesDecimalBoundariesForMinimumDecimalValue()
        {
            // Setup
            string value = "-79228162514264337593543950335";
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            var expected = decimal.Parse(value,
                System.Globalization.CultureInfo.InvariantCulture);
            var stored = handler.Set(expected,
                null);

            // Act
            var actual = handler.Get(stored,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Round-trips the full decimal range through the driver type.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesDecimalBoundariesForSmallestPositiveDecimalValue()
        {
            // Setup
            string value = "0.0000000000000000000000000001";
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            var expected = decimal.Parse(value,
                System.Globalization.CultureInfo.InvariantCulture);
            var stored = handler.Set(expected,
                null);

            // Act
            var actual = handler.Get(stored,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Rejects driver values beyond the decimal range.
        /// </summary>
        [TestMethod]
        public void GetWideDriverDecimalThrowsOverflow()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();
            var textHandler = new ClickHouseDecimalToStringPropertyHandler();
            var stored = textHandler.Set("99999999999999999999999999999999999999",
                null);

            // Act
            Action action = () => handler.Get(stored,
                null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(action);
        }

        /// <summary>
        /// Preserves absent values when writing.
        /// </summary>
        [TestMethod]
        public void SetNullReturnsNull()
        {
            // Setup
            var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

            // Act
            var actual = handler.Set(null,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}