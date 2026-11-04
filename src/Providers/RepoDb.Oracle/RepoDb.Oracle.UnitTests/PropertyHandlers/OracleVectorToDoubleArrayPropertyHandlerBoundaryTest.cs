using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.Oracle;
using System;

namespace RepoDb.Oracle.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class OracleVectorToDoubleArrayPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves an absent vector.
        /// </summary>
        [TestMethod]
        public void GetNullReturnsNull()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();

            // Act
            var actual = handler.Get(null,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves a database null vector.
        /// </summary>
        [TestMethod]
        public void GetDbNullReturnsNull()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves an absent vector when writing.
        /// </summary>
        [TestMethod]
        public void SetNullReturnsNull()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();

            // Act
            var actual = handler.Set(null,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Passes through a matching numeric array.
        /// </summary>
        [TestMethod]
        public void GetTypedArrayPreservesReference()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();
            var expected = new double[] { 1, -2 };

            // Act
            var actual = handler.Get(expected,
                null);

            // Assert
            Assert.AreSame(expected,
                actual);
        }

        /// <summary>
        /// Rejects an unsupported vector representation.
        /// </summary>
        [TestMethod]
        public void GetUnsupportedInputThrows()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();

            // Act
            Action action = () => handler.Get(42,
                null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        /// <summary>
        /// Round-trips numeric vector elements.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesElements()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();
            var expected = new double[] { 1, -2 };
            var stored = handler.Set(expected,
                null);

            // Act
            var actual = handler.Get(stored,
                null);

            // Assert
            CollectionAssert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Converts between floating point vector representations.
        /// </summary>
        [TestMethod]
        public void GetConvertsOtherNumericArray()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();
            var expected = new double[] { 1, -2 };

            // Act
            var actual = handler.Get(new float[] { 1, -2 },
                null);

            // Assert
            CollectionAssert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Reads numeric elements from JSON text.
        /// </summary>
        [TestMethod]
        public void GetJsonReturnsElements()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();
            var expected = new double[] { 1, -2 };

            // Act
            var actual = handler.Get("[1,-2]",
                null);

            // Assert
            CollectionAssert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Treats empty vector text as absent.
        /// </summary>
        [TestMethod]
        public void GetEmptyTextReturnsNull()
        {
            // Setup
            var handler = new OracleVectorToDoubleArrayPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty,
                null);

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}