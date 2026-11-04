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
        /// <param name="value">The signed byte.</param>
        /// <param name="expected">The expected boolean.</param>
        [DataRow((sbyte)0,
            false)]
        [DataRow((sbyte)1,
            true)]
        [DataRow(sbyte.MinValue,
            true)]
        [DataRow(sbyte.MaxValue,
            true)]
        [TestMethod]
        public void GetTreatsEveryNonzeroValueAsTrue(sbyte value,
            bool expected)
        {
            // Setup
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
        /// <param name="value">The boolean.</param>
        /// <param name="expected">The expected byte.</param>
        [DataRow(false,
            (sbyte)0)]
        [DataRow(true,
            (sbyte)1)]
        [TestMethod]
        public void SetProducesCanonicalByte(bool value,
            sbyte expected)
        {
            // Setup
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