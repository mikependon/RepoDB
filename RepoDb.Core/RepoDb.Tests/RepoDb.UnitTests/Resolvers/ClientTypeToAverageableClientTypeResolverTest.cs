using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToAverageableClientTypeResolverTest
    {
        private readonly ClientTypeToAverageableClientTypeResolver m_resolver = new ClientTypeToAverageableClientTypeResolver();

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForShort()
        {
            // Setup
            var clientType = typeof(short);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForInt()
        {
            // Setup
            var clientType = typeof(int);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForLong()
        {
            // Setup
            var clientType = typeof(long);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForUInt16()
        {
            // Setup
            var clientType = typeof(UInt16);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForUInt32()
        {
            // Setup
            var clientType = typeof(UInt32);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClientTypeToAverageableClientTypeResolverForUInt64()
        {
            // Setup
            var clientType = typeof(UInt64);

            // Act
            var result = m_resolver.Resolve(clientType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
