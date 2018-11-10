using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Linq;
using System.Text;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QueryGroupTest
    {
        #region Expressions

        public class QueryGroupTestClassMember
        {
            public string PropertyString { get; set; }
        }

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
            public QueryGroupTestClassMember Member { get; set; }
        }

        private int GetIntValueForParseExpression()
        {
            return 1;
        }

        private string GetStringValueForParseExpression()
        {
            return "ABC";
        }

        private double GetDoubleValueForParseExpression()
        {
            return 1234567.12;
        }

        private DateTime GetDateTimeValueForParseExpression()
        {
            return new DateTime(2018, 01, 01, 2, 4, 11, 112);
        }

        private float GetSingleValueForParseExpression()
        {
            return 18891;
        }

        private Guid GetGuidValueForParseExpression()
        {
            return Guid.Parse("4C43B849-4FD1-4E7D-95CA-A0EE0D358DE7");
        }

        private bool GetBooleanValueForParseExpression()
        {
            return 1 != 0;
        }

        private Byte[] GetBytesValueForParseExpression()
        {
            return Encoding.UTF8.GetBytes(GetStringValueForParseExpression());
        }

        // Name

        [TestMethod]
        public void TestParseExpressionWithNameAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1);

            // Act
            var actual = parsed.QueryFields.First().Field.Name;
            var expected = "PropertyInt";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithNameAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => 1 == e.PropertyInt);

            // Act
            var actual = parsed.QueryFields.First().Field.Name;
            var expected = "PropertyInt";

            // Assert
            Assert.AreEqual(expected, actual);
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
        public void TestParseExpressionWithIntMathOperations()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == (1 + 1)).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithIntNewClassInstance()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == new Random().Next(int.MaxValue)).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithIntMethodClass()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == Convert.ToInt32("1000")).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private string TestParseExpressionWithIntArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyInt == value).GetString();

            // Return
            return actual;
        }

        [TestMethod]
        public void TestParseExpressionWithIntArgumentParameter()
        {
            // Act
            var actual = TestParseExpressionWithIntArgumentParameterMethod<QueryGroupTestClass>(1);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Contains

        [TestMethod]
        public void TestParseExpressionContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => !(new int[] { 1, 2 }.Contains(e.PropertyInt)));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => !list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsFromVariableEqualsTrue()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => list.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsFromVariableEqualsFalse()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => list.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Test (Expression Value)

        [TestMethod]
        public void TestParseExpressionValueIntConstant()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == 1);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueIntVariable()
        {
            // Setup
            var value = 1;
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueIntClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyInt = 1
            };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value.PropertyInt);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value.PropertyInt;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueIntMethodCall()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == GetIntValueForParseExpression());

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = GetIntValueForParseExpression();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueIntVariableMethodCall()
        {
            // Setup
            var value = GetIntValueForParseExpression();
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == value);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithIntMathOperations()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == (1 + 1));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithIntNewClassInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == new Random().Next(int.MaxValue));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value.GetType();
            var expected = typeof(int);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithIntMethodClass()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == Convert.ToInt32("1000"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1000;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithIntArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyInt == value);

            // Return
            return actual;
        }

        [TestMethod]
        public void TestParseExpressionValueWithIntArgumentParameter()
        {
            // Setup
            var parsed = TestParseExpressionValueWithIntArgumentParameterMethod<QueryGroupTestClass>(1);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueStringConstant()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == "ABC");

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueStringVariable()
        {
            // Setup
            var value = Guid.NewGuid().ToString();
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueStringClassProperty()
        {
            // Setup
            var value = new QueryGroupTestClass
            {
                PropertyString = Guid.NewGuid().ToString()
            };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value.PropertyString);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value.PropertyString;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueStringMethodCall()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == GetStringValueForParseExpression());

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value.GetType();
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueStringVariableMethodCall()
        {
            // Setup
            var value = GetStringValueForParseExpression();
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == value);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringConcatenation()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == ("A" + "B" + "C"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringNewClassInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == (new QueryGroupTestClass() { PropertyString = "ABC" }).PropertyString);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringMethodClass()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == Convert.ToString("ABC"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithStringArgumentParameterMethod<TEntity>(string value) where TEntity : QueryGroupTestClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyString == value);

            // Return
            return actual;
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringArgumentParameter()
        {
            // Setup
            var parsed = TestParseExpressionValueWithStringArgumentParameterMethod<QueryGroupTestClass>("ABC");

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringNewClassInstanceMemberSetVariable()
        {
            // Setup
            var member = new QueryGroupTestClassMember() { PropertyString = "ABC" };
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == (new QueryGroupTestClass() { Member = member }).Member.PropertyString);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringNewClassInstanceMemberSetInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == (new QueryGroupTestClass() { Member = new QueryGroupTestClassMember() { PropertyString = "ABC" } }).Member.PropertyString);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringConditionalChecking()
        {
            // Setup
            var value = "ABC";
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyString == (value != null ? value : null));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueWithStringConditionalCheckingOpposite()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == (1 == 2 ? 1 : 2));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDefaultValue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestClass>(e => e.PropertyInt == default(int));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = default(int);

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
