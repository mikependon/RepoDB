using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.SqlServer;
using System;

namespace RepoDb.SqlServer.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class SqlServerVectorToFloatArrayPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves an absent vector.
        /// </summary>
        [TestMethod]
        public void GetNullReturnsNull()
        {
            // Setup
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();


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
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();


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
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();


            // Act
            var actual = handler.Set(null,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Serializes vector elements as invariant JSON.
        /// </summary>
        [TestMethod]
        public void SetProducesInvariantJson()
        {
            // Setup
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();


            // Act
            var actual = handler.Set(new float[] { 1.25f, -2f },
                null);


            // Assert
            Assert.AreEqual("[1.25,-2]",
                actual);
        }

        /// <summary>
        /// Serializes an empty vector as an empty JSON array.
        /// </summary>
        [TestMethod]
        public void SetEmptyVectorProducesEmptyArray()
        {
            // Setup
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();


            // Act
            var actual = handler.Set(Array.Empty<float>(),
                null);


            // Assert
            Assert.AreEqual("[]",
                actual);
        }

        /// <summary>
        /// Reads the numeric elements of a driver vector.
        /// </summary>
        [TestMethod]
        public void GetDriverVectorReturnsElements()
        {
            // Setup
            var handler = new SqlServerVectorToFloatArrayPropertyHandler();
            var expected = new float[] { 1.25f, -2f };
            var vector = new Microsoft.Data.SqlTypes.SqlVector<float>(expected);


            // Act
            var actual = handler.Get(vector,
                null);


            // Assert
            CollectionAssert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}