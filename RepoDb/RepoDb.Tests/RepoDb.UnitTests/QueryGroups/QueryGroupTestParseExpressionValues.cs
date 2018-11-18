using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Values

        [TestMethod]
        public void TestParseExpressionValueIntConstant()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value);

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
            var value = new QueryGroupTestExpressionClass
            {
                PropertyInt = 1
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value.PropertyInt);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == GetIntValueForParseExpression());

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == (1 + 1));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == new Random().Next(int.MaxValue));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == Convert.ToInt32("1000"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1000;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithIntArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestExpressionClass
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
            var parsed = TestParseExpressionValueWithIntArgumentParameterMethod<QueryGroupTestExpressionClass>(1);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == "ABC");

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value);

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
            var value = new QueryGroupTestExpressionClass
            {
                PropertyString = Guid.NewGuid().ToString()
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value.PropertyString);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == GetStringValueForParseExpression());

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == ("A" + "B" + "C"));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { PropertyString = "ABC" }).PropertyString);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == Convert.ToString("ABC"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithStringArgumentParameterMethod<TEntity>(string value) where TEntity : QueryGroupTestExpressionClass
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
            var parsed = TestParseExpressionValueWithStringArgumentParameterMethod<QueryGroupTestExpressionClass>("ABC");

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
            var member = new QueryGroupTestExpressionClassMember() { PropertyString = "ABC" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { Member = member }).Member.PropertyString);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { Member = new QueryGroupTestExpressionClassMember() { PropertyString = "ABC" } }).Member.PropertyString);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (value != null ? value : null));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == (1 == 2 ? 1 : 2));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == default(int));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = default(int);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Contains

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyContainsWhereValueHasWildcardsAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("%A"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyContainsWhereValueHasWildcardsAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A%"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyStartsWith()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyStartsWithWhereValueHasWildcardsAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A%"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyEndsWith()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForStringPropertyEndsWithWhereValueHasWildcardsAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("%A"));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { "A", "B" }).Contains(e.PropertyString));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { "A", "B" }).Contains(e.PropertyString));

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { "A", "B" }).Contains(e.PropertyString) == false);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { "A", "B" }).Contains(e.PropertyString) == false);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        // All

        [TestMethod]
        public void TestParseExpressionValueForArrayAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString));

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayAllEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString) == false);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayAllEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString) == true);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => !(new[] { "A", "B" }).All(s => s == p.PropertyString));

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }
        [TestMethod]
        public void TestParseExpressionValueForArrayNotAllEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s == p.PropertyString) == false);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotAllEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s == p.PropertyString) == true);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        // Any

        [TestMethod]
        public void TestParseExpressionValueForArrayAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString));

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayAnyEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString) == false);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayAnyEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString) == true);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => !(new[] { "A", "B" }).Any(s => s == p.PropertyString));

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }
        [TestMethod]
        public void TestParseExpressionValueForArrayNotAnyEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s == p.PropertyString) == false);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestParseExpressionValueForArrayNotAnyEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s == p.PropertyString) == true);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }
    }
}
