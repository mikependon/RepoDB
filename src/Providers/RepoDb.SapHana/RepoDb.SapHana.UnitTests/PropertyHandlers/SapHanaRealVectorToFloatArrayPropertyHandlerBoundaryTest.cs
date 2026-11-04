using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.SapHana;
using System;

namespace RepoDb.SapHana.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class SapHanaRealVectorToFloatArrayPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves an absent vector.
        /// </summary>
        [TestMethod]
        public void GetNullReturnsNull()
        {
            // Setup
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

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
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

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
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

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
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
            var expected = new float[] { 1, -2 };

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
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

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
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();
            var expected = new float[] { 1, -2 };
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
        /// Reads a vector without a dimension header.
        /// </summary>
        [TestMethod]
        public void GetBytesReadsRawVector()
        {
            // Setup
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 0, 0, 128, 63, 0, 0, 0, 192 },
                null);

            // Assert
            CollectionAssert.AreEqual(new float[] { 1, -2 },
                actual);
        }

        /// <summary>
        /// Reads a vector with a dimension header.
        /// </summary>
        [TestMethod]
        public void GetBytesReadsDimensionHeader()
        {
            // Setup
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 2, 0, 0, 0, 0, 0, 128, 63, 0, 0, 0, 192 },
                null);

            // Assert
            CollectionAssert.AreEqual(new float[] { 1, -2 },
                actual);
        }

        /// <summary>
        /// Reads an empty binary vector.
        /// </summary>
        [TestMethod]
        public void GetEmptyBytesReturnsEmptyVector()
        {
            // Setup
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(Array.Empty<byte>(),
                null);

            // Assert
            Assert.HasCount(0,
                actual);
        }

        /// <summary>
        /// Rejects incomplete floating point values.
        /// </summary>
        [TestMethod]
        public void GetMisalignedBytesThrowsForOne()
        {
            // Setup
            int length = 1;
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            Action action = () => handler.Get(new byte[length],
                null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        /// <summary>
        /// Rejects incomplete floating point values.
        /// </summary>
        [TestMethod]
        public void GetMisalignedBytesThrowsForThree()
        {
            // Setup
            int length = 3;
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            Action action = () => handler.Get(new byte[length],
                null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        /// <summary>
        /// Rejects incomplete floating point values.
        /// </summary>
        [TestMethod]
        public void GetMisalignedBytesThrowsForFive()
        {
            // Setup
            int length = 5;
            var handler = new SapHanaRealVectorToFloatArrayPropertyHandler();

            // Act
            Action action = () => handler.Get(new byte[length],
                null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        #endregion
    }
}