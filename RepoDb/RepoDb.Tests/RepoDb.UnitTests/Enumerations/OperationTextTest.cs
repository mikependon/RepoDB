using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class OperationTextTest
    {
        private TextAttribute GetOperationTextAttribute(Operation operation)
        {
            return typeof(Operation)
                .GetMembers()
                .First(member => member.Name.ToLower() == operation.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
        }

        [Test]
        public void TestEqual()
        {
            // Prepare
            var operation = Operation.Equal;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("=", parsed.Text);
        }

        [Test]
        public void TestNotEqual()
        {
            // Prepare
            var operation = Operation.NotEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<>", parsed.Text);
        }

        [Test]
        public void TestLessThan()
        {
            // Prepare
            var operation = Operation.LessThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<", parsed.Text);
        }

        [Test]
        public void TestGreaterThan()
        {
            // Prepare
            var operation = Operation.GreaterThan;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">", parsed.Text);
        }

        [Test]
        public void TestLessThanOrEqual()
        {
            // Prepare
            var operation = Operation.LessThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("<=", parsed.Text);
        }

        [Test]
        public void TestGreaterThanOrEqual()
        {
            // Prepare
            var operation = Operation.GreaterThanOrEqual;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual(">=", parsed.Text);
        }

        [Test]
        public void TestLike()
        {
            // Prepare
            var operation = Operation.Like;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("LIKE", parsed.Text);
        }

        [Test]
        public void TestNotLike()
        {
            // Prepare
            var operation = Operation.NotLike;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT LIKE", parsed.Text);
        }

        [Test]
        public void TestBetween()
        {
            // Prepare
            var operation = Operation.Between;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("BETWEEN", parsed.Text);
        }

        [Test]
        public void TestNotBetween()
        {
            // Prepare
            var operation = Operation.NotBetween;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT BETWEEN", parsed.Text);
        }

        [Test]
        public void TestIn()
        {
            // Prepare
            var operation = Operation.In;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("IN", parsed.Text);
        }

        [Test]
        public void TestNotIn()
        {
            // Prepare
            var operation = Operation.NotIn;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("NOT IN", parsed.Text);
        }

        [Test]
        public void TestAll()
        {
            // Prepare
            var operation = Operation.All;

            // Act
            var parsed = GetOperationTextAttribute(operation);

            // Assert
            Assert.AreEqual("AND", parsed.Text);
        }

        [Test]
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
