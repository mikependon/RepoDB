#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Requests;

namespace RepoDb.UnitTests.Requests
{
    [TestClass]
    public class BaseRequestTest
    {
        [TestMethod]
        public void TestNullObject()
        {
            // Setup
            var request = new FirstRequest();

            // Act
            var actual = request.Equals((object)null);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestNullRequest()
        {
            // Setup
            var request = new FirstRequest();

            // Act
            var actual = request.Equals((BaseRequest)null);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSameRequestType()
        {
            // Setup
            var request = new FirstRequest();
            var other = new FirstRequest();

            // Act
            var actual = request.Equals(other);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSameRequestTypeAsObject()
        {
            // Setup
            var request = new FirstRequest();
            object other = new FirstRequest();

            // Act
            var actual = request.Equals(other);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDifferentRequestType()
        {
            // Setup
            var request = new FirstRequest();
            var other = new SecondRequest();

            // Act
            var actual = request.Equals(other);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestHashCodeIsStable()
        {
            // Setup
            var request = new FirstRequest();
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestEqualNullOperands()
        {
            // Setup
            BaseRequest left = null;
            BaseRequest right = null;

            // Act
            var actual = left == right;

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestNullLeftOperand()
        {
            // Setup
            BaseRequest left = null;
            BaseRequest right = new FirstRequest();

            // Act
            var actual = left == right;

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestNullRightOperand()
        {
            // Setup
            BaseRequest left = new FirstRequest();
            BaseRequest right = null;

            // Act
            var actual = left == right;

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestEqualOperands()
        {
            // Setup
            BaseRequest left = new FirstRequest();
            BaseRequest right = new FirstRequest();

            // Act
            var actual = left == right;

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUnequalOperands()
        {
            // Setup
            BaseRequest left = new FirstRequest();
            BaseRequest right = new SecondRequest();

            // Act
            var actual = left != right;

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestEqualOperandsAreNotUnequal()
        {
            // Setup
            BaseRequest left = new FirstRequest();
            BaseRequest right = new FirstRequest();

            // Act
            var actual = left != right;

            // Assert
            Assert.IsFalse(actual);
        }

        private sealed class FirstRequest : BaseRequest
        {
            public FirstRequest() : base("Table", null, null) { }
        }

        private sealed class SecondRequest : BaseRequest
        {
            public SecondRequest() : base("Table", null, null) { }
        }
    }
}
