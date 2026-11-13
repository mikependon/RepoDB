#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Reflection;
using RepoDb.UnitTests.CustomObjects;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.UnitTests.Compilers
{
    [TestClass]
    public class CompilerJsonTest
    {
        #region IsJsonType

        [TestMethod]
        public void TestCompilerIsJsonTypeWithJsonValue()
        {
            // Act
            var result = Compiler.IsJsonType(typeof(DbJsonValue<JsonSampleDocument>));

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompilerIsJsonTypeWithString()
        {
            // Act
            var result = Compiler.IsJsonType(typeof(string));

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCompilerIsJsonTypeWithNull()
        {
            // Act
            var result = Compiler.IsJsonType(null);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region IsJsonValue

        [TestMethod]
        public void TestCompilerIsJsonValueWithJsonProperty()
        {
            // Setup
            var property = PropertyCache.Get<JsonSampleEntity>().First(p => p.PropertyInfo.Name == nameof(JsonSampleEntity.Document));

            // Act
            var result = Compiler.IsJsonValue(property);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompilerIsJsonValueWithPlainProperty()
        {
            // Setup
            var property = PropertyCache.Get<JsonSampleEntity>().First(p => p.PropertyInfo.Name == nameof(JsonSampleEntity.Id));

            // Act
            var result = Compiler.IsJsonValue(property);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCompilerIsJsonValueWithNull()
        {
            // Act
            var result = Compiler.IsJsonValue(null);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetDbJsonParameterValueAssignmentExpression

        [TestMethod]
        public void TestCompilerGetDbJsonParameterValueAssignmentExpression()
        {
            // Setup
            var parameter = Expression.Variable(typeof(System.Data.Common.DbParameter));
            var value = Expression.Constant(null, typeof(object));

            // Act
            var expression = (MethodCallExpression)Compiler.GetDbJsonParameterValueAssignmentExpression(parameter, value, new CustomDbSetting());

            // Assert
            Assert.AreEqual("SetValue", expression.Method.Name);
        }

        #endregion

        #region ConvertExpressionWithAutomaticConversion

        [TestMethod]
        public void TestCompilerConvertExpressionWithAutomaticConversionFromJsonValue()
        {
            // Setup
            var expression = Expression.Constant(default(DbJsonValue<JsonSampleDocument>));

            // Act
            var converted = Compiler.ConvertExpressionWithAutomaticConversion(expression, typeof(string));

            // Assert
            Assert.AreSame(expression, converted);
        }

        [TestMethod]
        public void TestCompilerConvertExpressionWithAutomaticConversionToJsonValue()
        {
            // Setup
            var expression = Expression.Constant("{}");

            // Act
            var converted = Compiler.ConvertExpressionWithAutomaticConversion(expression, typeof(DbJsonValue<JsonSampleDocument>));

            // Assert
            Assert.AreSame(expression, converted);
        }

        #endregion
    }
}
