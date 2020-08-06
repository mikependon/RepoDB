using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OperationTextTest
    {
        private TextAttribute GetOperationTextAttribute(Operation operation)
        {
            return typeof(Operation)
                .GetMembers()
                .First(member => member.Name.ToLowerInvariant() == operation.ToString().ToLowerInvariant())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestOperationEqualText()
        {
            // Prepare
            var operation = Operation.Equal;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("=", parsed.Text);
        }

        [TestMethod]
        public void TestOperationNotEqualText()
        {
            // Prepare
            var operation = Operation.NotEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<>", parsed.Text);
        }

        [TestMethod]
        public void TestOperationLessThanText()
        {
            // Prepare
            var operation = Operation.LessThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<", parsed.Text);
        }

        [TestMethod]
        public void TestOperationGreaterThanText()
        {
            // Prepare
            var operation = Operation.GreaterThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">", parsed.Text);
        }

        [TestMethod]
        public void TestOperationLessThanOrEqualText()
        {
            // Prepare
            var operation = Operation.LessThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<=", parsed.Text);
        }

        [TestMethod]
        public void TestOperationGreaterThanOrEqualText()
        {
            // Prepare
            var operation = Operation.GreaterThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">=", parsed.Text);
        }

        [TestMethod]
        public void TestOperationLikeText()
        {
            // Prepare
            var operation = Operation.Like;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("LIKE", parsed.Text);
        }

        [TestMethod]
        public void TestOperationNotLikeText()
        {
            // Prepare
            var operation = Operation.NotLike;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT LIKE", parsed.Text);
        }

        [TestMethod]
        public void TestOperationBetweenText()
        {
            // Prepare
            var operation = Operation.Between;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("BETWEEN", parsed.Text);
        }

        [TestMethod]
        public void TestOperationNotBetweenText()
        {
            // Prepare
            var operation = Operation.NotBetween;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT BETWEEN", parsed.Text);
        }

        [TestMethod]
        public void TestOperationInText()
        {
            // Prepare
            var operation = Operation.In;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("IN", parsed.Text);
        }

        [TestMethod]
        public void TestOperationNotInText()
        {
            // Prepare
            var operation = Operation.NotIn;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT IN", parsed.Text);
        }
    }
}
