using RepoDb.Attributes;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        #region ParameterPropertyValueSetterAttribute

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static IEnumerable<Expression> GetParameterPropertyValueSetterAttributesAssignmentExpressions(
            ParameterExpression parameterVariable,
            ClassProperty classProperty) =>
            GetParameterPropertyValueSetterAttributesAssignmentExpressions((Expression)parameterVariable, classProperty);

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static IEnumerable<Expression> GetParameterPropertyValueSetterAttributesAssignmentExpressions(
            Expression parameterVariable,
            ClassProperty classProperty)
        {
            var attributes = GetParameterPropertyValueSetterAttributes(classProperty);
            if (attributes?.Any() != true)
            {
                return default;
            }

            var expressions = new List<Expression>();

            foreach (var attribute in attributes)
            {
                var expression = GetParameterPropertyValueSetterAttributesAssignmentExpression(parameterVariable,
                    attribute);
                expressions.AddIfNotNull(expression);
            }

            return expressions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal static Expression GetParameterPropertyValueSetterAttributesAssignmentExpression(
            ParameterExpression parameterVariableExpression,
            ParameterPropertyValueSetterAttribute attribute) =>
            GetParameterPropertyValueSetterAttributesAssignmentExpression((Expression)parameterVariableExpression, attribute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal static Expression GetParameterPropertyValueSetterAttributesAssignmentExpression(
            Expression parameterExpression,
            ParameterPropertyValueSetterAttribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }

            // The problem to this is because of the possibilities of multiple attributes configured for 
            // DB multiple providers within a single entity and if the parameterExpression is not really
            // covertible to the target attriute.ParameterType

            //return Expression.Call(Expression.Convert(parameterExpression, attribute.ParameterType),
            //    attribute.PropertyInfo.SetMethod,
            //    Expression.Constant(attribute.Value));

            var method = GetParameterPropertyValueSetterAttributeSetValueMethod();
            return Expression.Call(Expression.Constant(attribute), method, parameterExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetParameterPropertyValueSetterAttributeSetValueMethod() =>

            StaticType.ParameterPropertyValueSetterAttribute.GetMethod("SetValue",
                BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        private static IEnumerable<ParameterPropertyValueSetterAttribute> GetParameterPropertyValueSetterAttributes(ClassProperty classProperty) =>
            classProperty?
                .PropertyInfo?
                .GetCustomAttributes()?
                .Where(e =>
                    StaticType.ParameterPropertyValueSetterAttribute.IsAssignableFrom(e.GetType()))
                .Select(e =>
                    (ParameterPropertyValueSetterAttribute)e);

        #endregion
    }
}
