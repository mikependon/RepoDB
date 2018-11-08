using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Linq.Expressions;
using System.Text;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QueryGroupTest
    {
        #region Expressions

        public class QueryGroupTestClass
        {
            public int PropertyInt { get; set; }
            public string PropertyString { get; set; }
            public double PropertyDouble { get; set; }
            public DateTime PropertyDateTime { get; set; }
            public float PropertySingle { get; set; }
            public Guid PropertyGuid { get; set; }
            public Boolean PropertyBoolean { get; set; }
            public Byte[] PropertyBytes { get; set; }
        }

        private int GetIntValueForParseExpression()
        {
            return new Random().Next(int.MaxValue);
        }

        private string GetStringValueForParseExpression()
        {
            return Guid.NewGuid().ToString();
        }

        private double GetDoubleValueForParseExpression()
        {
            return new Random().NextDouble() * double.MaxValue;
        }

        private DateTime GetDateTimeValueForParseExpression()
        {
            return DateTime.UtcNow.AddSeconds(-new Random().Next(1, 60 * 60 * 24 * 30));
        }

        private float GetSingleValueForParseExpression()
        {
            return Convert.ToSingle(new Random().NextDouble() * 1000);
        }

        private Guid GetGuidValueForParseExpression()
        {
            return Guid.NewGuid();
        }

        private bool GetBooleanValueForParseExpression()
        {
            return new Random().NextDouble() > 0.5;
        }

        private Byte[] GetBytesValueForParseExpression()
        {
            return Encoding.UTF8.GetBytes(GetStringValueForParseExpression());
        }

        // Int

        [TestMethod]
        public void TestParseExpressionIntConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionIntVariable()
        {
            // Setup
            var value = 1;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionIntClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyInt = 1
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value.PropertyInt).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionIntMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == GetIntValueForParseExpression()).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionIntVariableMethodCall()
        {
            // Setup
            var value = GetIntValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // String

        [TestMethod]
        public void TestParseExpressionStringConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == "A").GetString();
            var expected = "([PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionStringVariable()
        {
            // Setup
            var value = "A";

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value).GetString();
            var expected = "([PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionStringClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyString = "A"
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value.PropertyString).GetString();
            var expected = "([PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionStringMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == GetStringValueForParseExpression()).GetString();
            var expected = "([PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionStringVariableMethodCall()
        {
            // Setup
            var value = GetStringValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value).GetString();
            var expected = "([PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Double

        [TestMethod]
        public void TestParseExpressionDoubleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDouble == 1.0).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleVariable()
        {
            // Setup
            var value = 1.0;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDouble == value).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyDouble = 1.0
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDouble == value.PropertyDouble).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDouble == GetDoubleValueForParseExpression()).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleVariableMethodCall()
        {
            // Setup
            var value = GetDoubleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDouble == value).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // DateTime

        [TestMethod]
        public void TestParseExpressionDateTimeConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDateTime == DateTime.UtcNow).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeVariable()
        {
            // Setup
            var value = DateTime.UtcNow;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDateTime == value).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyDateTime = DateTime.UtcNow
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDateTime == value.PropertyDateTime).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDateTime == GetDateTimeValueForParseExpression()).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeVariableMethodCall()
        {
            // Setup
            var value = GetDateTimeValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyDateTime == value).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Single

        [TestMethod]
        public void TestParseExpressionSingleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertySingle == 1).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleVariable()
        {
            // Setup
            var value = 1;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertySingle == value).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertySingle = 1
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertySingle == value.PropertySingle).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertySingle == GetSingleValueForParseExpression()).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleVariableMethodCall()
        {
            // Setup
            var value = GetSingleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertySingle == value).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Guid

        [TestMethod]
        public void TestParseExpressionGuidConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyGuid == Guid.NewGuid()).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidVariable()
        {
            // Setup
            var value = Guid.NewGuid();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyGuid == value).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyGuid = Guid.NewGuid()
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyGuid == value.PropertyGuid).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyGuid == GetGuidValueForParseExpression()).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidVariableMethodCall()
        {
            // Setup
            var value = GetGuidValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyGuid == value).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Boolean

        [TestMethod]
        public void TestParseExpressionBooleanConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBoolean == true).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanVariable()
        {
            // Setup
            var value = true;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBoolean == value).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyBoolean = true
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBoolean == value.PropertyBoolean).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBoolean == GetBooleanValueForParseExpression()).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanVariableMethodCall()
        {
            // Setup
            var value = GetBooleanValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBoolean == value).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Byte

        [TestMethod]
        public void TestParseExpressionByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == new[] { byte.Parse("0") }).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionPassedByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == Encoding.Unicode.GetBytes("Test")).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteVariable()
        {
            // Setup
            var value = new[] { byte.Parse("0") };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == value).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyBytes = new[] { byte.Parse("0") }
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == value.PropertyBytes).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == GetBytesValueForParseExpression()).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteVariableMethodCall()
        {
            // Setup
            var value = GetBytesValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyBytes == value).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Operations

        [TestMethod]
        public void TestParseExpressionForEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionForNotEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt != 1).GetString();
            var expected = "([PropertyInt] <> @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionForGreaterThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt > 1).GetString();
            var expected = "([PropertyInt] > @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionForGreaterThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt >= 1).GetString();
            var expected = "([PropertyInt] >= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionForLessThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt < 1).GetString();
            var expected = "([PropertyInt] < @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionForLessThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt <= 1).GetString();
            var expected = "([PropertyInt] <= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionWithoutProperty()
        {
            QueryGroup.Parse<QueryGroupTestClass>(e => true).GetString();
        }

        // Properties

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1 && e.PropertyInt == 2).GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1 || e.PropertyInt == 2).GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Groupings

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1 && e.PropertyString == "A").GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1 || e.PropertyString == "A").GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOrAndSingleFieldForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) && e.PropertyString == "A").GetString();
            var expected = "([PropertyString] = @PropertyString AND ([PropertyInt] = @PropertyInt OR [PropertyDouble] = @PropertyDouble))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAndAndSingleFieldForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) || e.PropertyString == "A").GetString();
            var expected = "([PropertyString] = @PropertyString OR ([PropertyInt] = @PropertyInt AND [PropertyDouble] = @PropertyDouble))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) && (e.PropertyString == "A" && e.PropertySingle == 1)).GetString();
            var expected = "(([PropertyInt] = @PropertyInt AND [PropertyDouble] = @PropertyDouble) AND ([PropertyString] = @PropertyString AND [PropertySingle] = @PropertySingle))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) || (e.PropertyString == "A" || e.PropertySingle == 1)).GetString();
            var expected = "(([PropertyInt] = @PropertyInt OR [PropertyDouble] = @PropertyDouble) OR ([PropertyString] = @PropertyString OR [PropertySingle] = @PropertySingle))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Others

        [TestMethod]
        public void TestParseExpressionWithMathOperations()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == (1 + 1)).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithNewClassInstance()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == new Random().Next(int.MaxValue)).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithMethodClass()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == Convert.ToInt32("1000")).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private string TestParseExpressionWithArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyInt == value).GetString();

            // Return
            return actual;
        }

        [TestMethod]
        public void TestParseExpressionWithArgumentParameter()
        {
            // Act
            var actual = TestParseExpressionWithArgumentParameterMethod<QueryGroupTestClass>(1);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Dynamics

        // Expression

        [TestMethod]
        public void TestParseDynamicSingleExpression()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicOnParseDynamicIfConjunctionIsNotAConjunctionType()
        {
            // Setup
            var expression = new { Conjunction = "NotAConjunctionType", Field1 = 1 };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod]
        public void TestParseDynamicMultipleExpressions()
        {
            // Setup
            var expression = new { Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2 AND [Field3] = @Field3)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseDynamicMultipleExpressionsForConjunctionOr()
        {
            // Setup
            var expression = new { Conjunction = Conjunction.Or, Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1 OR [Field2] = @Field2 OR [Field3] = @Field3)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // No Operation

        [TestMethod]
        public void TestParseDynamicNoOperation()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // No Operation for IS NULL

        [TestMethod]
        public void TestParseDynamicNoOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = (object)null };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equal

        [TestMethod]
        public void TestParseDynamicEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equal for IS NULL

        [TestMethod]
        public void TestParseDynamicEqualOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual

        [TestMethod]
        public void TestParseDynamicNotEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <> @Field1)"; // !=

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual of IS NOT NULL

        [TestMethod]
        public void TestParseDynamicNotEqualOperationForIsNotNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NOT NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThan

        [TestMethod]
        public void TestParseDynamicLessThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] < @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThan

        [TestMethod]
        public void TestParseDynamicGreaterThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] > @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThanOrEqual

        [TestMethod]
        public void TestParseDynamicLessThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThanOrEqual

        [TestMethod]
        public void TestParseDynamicGreaterThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] >= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Like

        [TestMethod]
        public void TestParseDynamicLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Like, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotLike

        [TestMethod]
        public void TestParseDynamicNotLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotLike, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] NOT LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Between

        [TestMethod]
        public void TestParseDynamicBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new[] { 1, 2 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesLengthIsLessThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesLengthIsGreaterThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // NotBetween

        [TestMethod]
        public void TestParseDynamicParseDynamicNotBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new[] { 1, 2 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] NOT BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValuesLengthIsNotEqualsTo2()
        {
            // Setup
            var expression1 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1 } } };
            var expression2 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression1);
            QueryGroup.Parse(expression2);
        }

        // In

        [TestMethod]
        public void TestParseDynamicParseDynamicInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new[] { 1, 2, 3 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // NotIn

        [TestMethod]
        public void TestParseDynamicNotInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new[] { 1, 2, 3 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // All

        [TestMethod]
        public void TestParseDynamicAllOperation()
        {
            // Setup
            var expression = new
            {
                Field1 = new
                {
                    Operation = Operation.All,
                    Value = new[]
                    {
                        new { Operation = Operation.Equal, Value = 1 },
                        new { Operation = Operation.NotEqual, Value = 1 }
                    }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            // Must be inside of another group as the ALL fields must be grouped by itself
            var expected = $"(([Field1] = @Field1 AND [Field1] <> @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfAllOperationValueIsNotAnExpressionOrAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfChildQueryGroupsAreNotAnExpressionsOrAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // Any

        [TestMethod]
        public void TestParseDynamicAnyOperation()
        {
            // Setup
            var expression = new
            {
                Field1 = new
                {
                    Operation = Operation.Any,
                    Value = new[]
                    {
                        new { Operation = Operation.Equal, Value = 1 },
                        new { Operation = Operation.NotEqual, Value = 1 }
                    }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            // Must be inside of another group as the ANY fields must be grouped by itself
            var expected = $"(([Field1] = @Field1 OR [Field1] <> @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfAnyOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Any, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // FixParameters

        [TestMethod]
        public void TestParseDynamicFixParametersOnASingleFieldWithMultipleExpression()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new dynamic[]
                {
                    new { Field1 = 2 },
                    new { Field1 = 3 },
                    new { Field1 = 4 }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field1] = @Field1_1) AND ([Field1] = @Field1_2) AND ([Field1] = @Field1_3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Others

        [TestMethod]
        public void TestParseDynamicChildQueryGroupsSingle()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new
                {
                    Field2 = 2,
                    Field3 = 3
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseDynamicChildQueryGroupsSingleForConjunctionOr()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new
                {
                    Conjunction = Conjunction.Or,
                    Field2 = 2,
                    Field3 = 3
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 OR [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseDynamicChildQueryGroupsMultiple()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new dynamic[]
                {
                    new
                    {
                        Field2 = 2,
                        Field3 = 3
                    },
                    new
                    {
                        Field4 = 2,
                        Field5 = 3
                    }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field3] = @Field3) AND ([Field4] = @Field4 AND [Field5] = @Field5))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
