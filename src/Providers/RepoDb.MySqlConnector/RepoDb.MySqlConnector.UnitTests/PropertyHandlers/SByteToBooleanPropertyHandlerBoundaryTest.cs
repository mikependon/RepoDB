using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.MySqlConnector.PropertyHandlers;
using System;

namespace RepoDb.MySqlConnector.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers property conversion boundaries.
    /// </summary>
    [TestClass]
    public class SByteToBooleanPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Converts signed byte boundary values to booleans.
        /// </summary>
        [TestMethod]
        public void GetTreatsEveryNonzeroValueAsTrueForSByteZero()
        {
            // Setup
            sbyte value = (sbyte)0;
            bool expected = false;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Get(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Converts signed byte boundary values to booleans.
        /// </summary>
        [TestMethod]
        public void GetTreatsEveryNonzeroValueAsTrueForSByteOne()
        {
            // Setup
            sbyte value = (sbyte)1;
            bool expected = true;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Get(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Converts signed byte boundary values to booleans.
        /// </summary>
        [TestMethod]
        public void GetTreatsEveryNonzeroValueAsTrueForSByteMinValue()
        {
            // Setup
            sbyte value = sbyte.MinValue;
            bool expected = true;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Get(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Converts signed byte boundary values to booleans.
        /// </summary>
        [TestMethod]
        public void GetTreatsEveryNonzeroValueAsTrueForSByteMaxValue()
        {
            // Setup
            sbyte value = sbyte.MaxValue;
            bool expected = true;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Get(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Writes zero or one for a boolean.
        /// </summary>
        [TestMethod]
        public void SetProducesCanonicalByteForFalse()
        {
            // Setup
            bool value = false;
            sbyte expected = (sbyte)0;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Set(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Writes zero or one for a boolean.
        /// </summary>
        [TestMethod]
        public void SetProducesCanonicalByteForTrue()
        {
            // Setup
            bool value = true;
            sbyte expected = (sbyte)1;
            var handler = new SByteToBooleanPropertyHandler();

            // Act
            var actual = handler.Set(value,
                null);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}