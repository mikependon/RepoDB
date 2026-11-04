using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;
using System;

namespace RepoDb.Vertica.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers property conversion boundaries.
    /// </summary>
    [TestClass]
    public class VerticaTimeToDateTimePropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves subsecond precision while removing the date.
        /// </summary>
        [TestMethod]
        public void GetRebasesDateAndPreservesTicks()
        {
            // Setup
            var handler = new VerticaTimeToDateTimePropertyHandler();
            var value = new DateTime(2026,
                11,
                4).AddTicks(123456789);


            // Act
            var actual = handler.Get(value,
                null);


            // Assert
            Assert.AreEqual(new DateTime(value.TimeOfDay.Ticks),
                actual);
        }

        /// <summary>
        /// Passes the original date and time to the driver.
        /// </summary>
        [TestMethod]
        public void SetPreservesValue()
        {
            // Setup
            var handler = new VerticaTimeToDateTimePropertyHandler();
            var expected = DateTime.MaxValue;


            // Act
            var actual = handler.Set(expected,
                null);


            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}