using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.DuckDb;
using System;

namespace RepoDb.DuckDb.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers property conversion boundaries.
    /// </summary>
    [TestClass]
    public class DuckDbStreamToByteArrayPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Passes an existing byte array through unchanged.
        /// </summary>
        [TestMethod]
        public void GetBytesPreservesReference()
        {
            // Setup
            var handler = new DuckDbStreamToByteArrayPropertyHandler();
            var expected = new byte[] { 1, 2, 3 };


            // Act
            var actual = handler.Get(expected,
                null);


            // Assert
            Assert.AreSame(expected,
                actual);
        }

        /// <summary>
        /// Reads from the current stream position.
        /// </summary>
        [TestMethod]
        public void GetStreamReadsRemainingBytes()
        {
            // Setup
            var handler = new DuckDbStreamToByteArrayPropertyHandler();
            using var stream = new System.IO.MemoryStream(new byte[] { 1, 2, 3 });
            stream.Position = 1;


            // Act
            var actual = handler.Get(stream,
                null);


            // Assert
            CollectionAssert.AreEqual(new byte[] { 2, 3 },
                actual);
        }

        /// <summary>
        /// Returns no bytes for an unsupported representation.
        /// </summary>
        [TestMethod]
        public void GetUnsupportedInputReturnsEmpty()
        {
            // Setup
            var handler = new DuckDbStreamToByteArrayPropertyHandler();


            // Act
            var actual = handler.Get(DBNull.Value,
                null);


            // Assert
            Assert.HasCount(0,
                actual);
        }

        /// <summary>
        /// Writes the original byte array.
        /// </summary>
        [TestMethod]
        public void SetPreservesReference()
        {
            // Setup
            var handler = new DuckDbStreamToByteArrayPropertyHandler();
            var expected = new byte[] { 1, 2, 3 };


            // Act
            var actual = handler.Set(expected,
                null);


            // Assert
            Assert.AreSame(expected,
                actual);
        }

        #endregion
    }
}