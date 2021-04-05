using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OperationTextTest
    {
        [TestMethod]
        public void TestOperationEqualText()
        {
            // Prepare
            var operation = Operation.Equal;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("=", text);
        }

        [TestMethod]
        public void TestOperationNotEqualText()
        {
            // Prepare
            var operation = Operation.NotEqual;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("<>", text);
        }

        [TestMethod]
        public void TestOperationLessThanText()
        {
            // Prepare
            var operation = Operation.LessThan;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("<", text);
        }

        [TestMethod]
        public void TestOperationGreaterThanText()
        {
            // Prepare
            var operation = Operation.GreaterThan;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual(">", text);
        }

        [TestMethod]
        public void TestOperationLessThanOrEqualText()
        {
            // Prepare
            var operation = Operation.LessThanOrEqual;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("<=", text);
        }

        [TestMethod]
        public void TestOperationGreaterThanOrEqualText()
        {
            // Prepare
            var operation = Operation.GreaterThanOrEqual;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual(">=", text);
        }

        [TestMethod]
        public void TestOperationLikeText()
        {
            // Prepare
            var operation = Operation.Like;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("LIKE", text);
        }

        [TestMethod]
        public void TestOperationNotLikeText()
        {
            // Prepare
            var operation = Operation.NotLike;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("NOT LIKE", text);
        }

        [TestMethod]
        public void TestOperationBetweenText()
        {
            // Prepare
            var operation = Operation.Between;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("BETWEEN", text);
        }

        [TestMethod]
        public void TestOperationNotBetweenText()
        {
            // Prepare
            var operation = Operation.NotBetween;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("NOT BETWEEN", text);
        }

        [TestMethod]
        public void TestOperationInText()
        {
            // Prepare
            var operation = Operation.In;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("IN", text);
        }

        [TestMethod]
        public void TestOperationNotInText()
        {
            // Prepare
            var operation = Operation.NotIn;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("NOT IN", text);
        }
    }
}
