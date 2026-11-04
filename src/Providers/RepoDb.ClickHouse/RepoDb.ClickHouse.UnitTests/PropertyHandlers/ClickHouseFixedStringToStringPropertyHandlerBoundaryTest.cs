using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.ClickHouse;
using System;

namespace RepoDb.ClickHouse.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers property conversion boundaries.
    /// </summary>
    [TestClass]
    public class ClickHouseFixedStringToStringPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves embedded null characters and removes padding.
        /// </summary>
        /// <param name="value">The padded input.</param>
        /// <param name="expected">The unpadded text.</param>
        [DataRow("text\0\0",
            "text")]
        [DataRow("\0\0",
            "")]
        [DataRow("a\0b\0",
            "a\0b")]
        [DataRow("",
            "")]
        [TestMethod]
        public void GetTextRemovesOnlyTrailingPadding(string value,
            string expected)
        {
            // Setup
            var handler = new ClickHouseFixedStringToStringPropertyHandler();


            // Act
            var actual = handler.Get(value,
                null);


            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Decodes multibyte text before trimming null padding.
        /// </summary>
        [TestMethod]
        public void GetBytesDecodesUtf8AndRemovesPadding()
        {
            // Setup
            var handler = new ClickHouseFixedStringToStringPropertyHandler();
            var bytes = System.Text.Encoding.UTF8.GetBytes("\u00E6\0\0");


            // Act
            var actual = handler.Get(bytes,
                null);


            // Assert
            Assert.AreEqual("\u00E6",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves database nulls.
        /// </summary>
        [TestMethod]
        public void GetDbNullReturnsNull()
        {
            // Setup
            var handler = new ClickHouseFixedStringToStringPropertyHandler();


            // Act
            var actual = handler.Get(DBNull.Value,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Rejects a nontext value.
        /// </summary>
        [TestMethod]
        public void GetUnsupportedValueThrows()
        {
            // Setup
            var handler = new ClickHouseFixedStringToStringPropertyHandler();


            // Act
            Action action = () => handler.Get(42,
                null);


            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        /// <summary>
        /// Leaves padding decisions to the server when writing.
        /// </summary>
        [TestMethod]
        public void SetPreservesPadding()
        {
            // Setup
            var handler = new ClickHouseFixedStringToStringPropertyHandler();
            const string value = "text\0";


            // Act
            var actual = handler.Set(value,
                null);


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}