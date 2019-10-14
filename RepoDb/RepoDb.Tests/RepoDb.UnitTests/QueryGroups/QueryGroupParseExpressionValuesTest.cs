using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        #region SubClass

        private static class StaticEnumClass
        {
            static StaticEnumClass()
            {
                Direction = GetDirection();
            }

            public static Direction Direction { get; }

            public static Direction GetDirection()
            {
                return Direction.East;
            }
        }

        private class NonStaticEnumClass
        {
            public NonStaticEnumClass()
            {
                Direction = GetDirection();
            }

            public Direction Direction { get; }

            public Direction GetDirection()
            {
                return Direction.East;
            }
        }

        #endregion

        #region Values

        [TestMethod]
        public void TestQueryGroupParseExpressionValueIntConstant()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueIntVariable()
        {
            // Setup
            var value = 1;
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueIntClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyInt = 1
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value.PropertyInt, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value.PropertyInt;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueIntMethodCall()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == GetIntValueForParseExpression(), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = GetIntValueForParseExpression();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueIntVariableMethodCall()
        {
            // Setup
            var value = GetIntValueForParseExpression();
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithIntMathOperations()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == (1 + 1), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithIntNewClassInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == new Random().Next(int.MaxValue), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value.GetType();
            var expected = typeof(int);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithIntMethodClass()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == Convert.ToInt32("1000"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1000;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithIntArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestExpressionClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyInt == value, Helper.DbSetting);

            // Return
            return actual;
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithIntArgumentParameter()
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
        public void TestQueryGroupParseExpressionValueStringConstant()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == "ABC", Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueStringVariable()
        {
            // Setup
            var value = Guid.NewGuid().ToString();
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueStringClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyString = Guid.NewGuid().ToString()
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value.PropertyString, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value.PropertyString;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueStringMethodCall()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == GetStringValueForParseExpression(), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value.GetType();
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueStringVariableMethodCall()
        {
            // Setup
            var value = GetStringValueForParseExpression();
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == value, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringConcatenation()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == ("A" + "B" + "C"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringNewClassInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { PropertyString = "ABC" }).PropertyString, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringMethodClass()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == Convert.ToString("ABC"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private QueryGroup TestParseExpressionValueWithStringArgumentParameterMethod<TEntity>(string value) where TEntity : QueryGroupTestExpressionClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyString == value, Helper.DbSetting);

            // Return
            return actual;
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringArgumentParameter()
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
        public void TestQueryGroupParseExpressionValueWithStringNewClassInstanceMemberSetVariable()
        {
            // Setup
            var member = new QueryGroupTestExpressionClassMember() { PropertyString = "ABC" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { Member = member }).Member.PropertyString, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringNewClassInstanceMemberSetInstance()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (new QueryGroupTestExpressionClass() { Member = new QueryGroupTestExpressionClassMember() { PropertyString = "ABC" } }).Member.PropertyString, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "ABC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringConditionalChecking()
        {
            // Setup
            var value = "ABC";
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString == (value != null ? value : null), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueWithStringConditionalCheckingOpposite()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == (1 == 2 ? 1 : 2), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionWithDefaultValue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == default(int), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = default(int);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Contains

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyContainsWhereValueHasWildcardsAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("%A"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyContainsWhereValueHasWildcardsAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A%"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyStartsWith()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyStartsWithWhereValueHasWildcardsAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A%"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyEndsWith()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForStringPropertyEndsWithWhereValueHasWildcardsAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("%A"), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "%A";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { "A", "B" }).Contains(e.PropertyString), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { "A", "B" }).Contains(e.PropertyString), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { "A", "B" }).Contains(e.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { "A", "B" }).Contains(e.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual("A", ((Array)actual).GetValue(0));
            Assert.AreEqual("B", ((Array)actual).GetValue(1));
        }

        #endregion

        #region All

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString), Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAllEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAllEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s != p.PropertyString) == true, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => !(new[] { "A", "B" }).All(s => s == p.PropertyString), Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }
        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAllEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s == p.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAllEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).All(s => s == p.PropertyString) == true, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        #endregion

        #region Any

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString), Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAnyEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayAnyEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s != p.PropertyString) == true, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => !(new[] { "A", "B" }).Any(s => s == p.PropertyString), Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }
        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAnyEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s == p.PropertyString) == false, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForArrayNotAnyEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => (new[] { "A", "B" }).Any(s => s == p.PropertyString) == true, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual("A", actual1);
            Assert.AreEqual("B", actual2);
        }

        #endregion

        #region Enums

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnum()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == Direction.East, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnums()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == Direction.East || p.Direction == Direction.West, Helper.DbSetting);

            // Act
            var actual1 = parsed.QueryGroups.First().QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryGroups.Last().QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual1);
            Assert.AreEqual(Direction.West, actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnumFromVariable()
        {
            // Setup
            var direction = Direction.East;
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == direction, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnumFromStaticClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == StaticEnumClass.GetDirection(), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnumFromStaticClassProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == StaticEnumClass.Direction, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnumFromNonStaticClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == new NonStaticEnumClass().GetDirection(), Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionValueForEnumFromNonStaticClassProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(p => p.Direction == new NonStaticEnumClass().Direction, Helper.DbSetting);

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.East, actual);
        }

        #endregion
    }
}
