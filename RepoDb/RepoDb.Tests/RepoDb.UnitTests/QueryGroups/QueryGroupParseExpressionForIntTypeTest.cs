using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Int

        [TestMethod]
        public void TestQueryGroupParseExpressionIntConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionIntVariable()
        {
            // Setup
            var value = 1;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionIntClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyInt = 1
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value.PropertyInt).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionIntMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == GetIntValueForParseExpression()).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionIntVariableMethodCall()
        {
            // Setup
            var value = GetIntValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == value).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Others

        [TestMethod]
        public void TestQueryGroupParseExpressionWithIntMathOperations()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == (1 + 1)).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionWithIntNewClassInstance()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == new Random().Next(int.MaxValue)).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionWithIntMethodClass()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == Convert.ToInt32("1000")).GetString(Helper.DbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private string TestParseExpressionWithIntArgumentParameterMethod<TEntity>(int value) where TEntity : QueryGroupTestExpressionClass
        {
            // Act
            var actual = QueryGroup.Parse<TEntity>(e => e.PropertyInt == value).GetString(Helper.DbSetting);

            // Return
            return actual;
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionWithIntArgumentParameter()
        {
            // Act
            var actual = TestParseExpressionWithIntArgumentParameterMethod<QueryGroupTestExpressionClass>(1);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
