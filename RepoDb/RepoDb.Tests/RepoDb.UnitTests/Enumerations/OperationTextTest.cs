using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class OperationTextTest
    {
        private TextAttribute GetOperationTextAttribute(Operation operation)
        {
            return typeof(Operation)
                .GetMembers()
                .First(member => member.Name.ToLower() == operation.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestEqual()
        {
            // Prepare
            var operation = Operation.Equal;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("=", parsed.Text);
        }

        [TestMethod]
        public void TestNotEqual()
        {
            // Prepare
            var operation = Operation.NotEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<>", parsed.Text);
        }

        [TestMethod]
        public void TestLessThan()
        {
            // Prepare
            var operation = Operation.LessThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<", parsed.Text);
        }

        [TestMethod]
        public void TestGreaterThan()
        {
            // Prepare
            var operation = Operation.GreaterThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">", parsed.Text);
        }

        [TestMethod]
        public void TestLessThanOrEqual()
        {
            // Prepare
            var operation = Operation.LessThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<=", parsed.Text);
        }

        [TestMethod]
        public void TestGreaterThanOrEqual()
        {
            // Prepare
            var operation = Operation.GreaterThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">=", parsed.Text);
        }

        [TestMethod]
        public void TestLike()
        {
            // Prepare
            var operation = Operation.Like;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("LIKE", parsed.Text);
        }

        [TestMethod]
        public void TestNotLike()
        {
            // Prepare
            var operation = Operation.NotLike;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT LIKE", parsed.Text);
        }

        [TestMethod]
        public void TestBetween()
        {
            // Prepare
            var operation = Operation.Between;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("BETWEEN", parsed.Text);
        }

        [TestMethod]
        public void TestNotBetween()
        {
            // Prepare
            var operation = Operation.NotBetween;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT BETWEEN", parsed.Text);
        }

        [TestMethod]
        public void TestIn()
        {
            // Prepare
            var operation = Operation.In;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("IN", parsed.Text);
        }

        [TestMethod]
        public void TestNotIn()
        {
            // Prepare
            var operation = Operation.NotIn;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT IN", parsed.Text);
        }

        [TestMethod]
        public void TestAll()
        {
            // Prepare
            var operation = Operation.All;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("AND", parsed.Text);
        }

        [TestMethod]
        public void TestAny()
        {
            // Prepare
            var operation = Operation.Any;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("OR", parsed.Text);
        }
    }
}
