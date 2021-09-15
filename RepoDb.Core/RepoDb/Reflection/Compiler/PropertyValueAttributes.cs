using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        #region PropertyValueAttribute

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static IEnumerable<Expression> GetPropertyValueAttributeAssignmentExpressions(
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
            var attributes = GetPropertyValueAttributes(classProperty);
            if (attributes?.Any() != true)
            {
                return default;
            }

            var expressions = new List<Expression>();

            foreach (var attribute in attributes)
            {
                if (attribute.IncludedInCompilation)
                {
                    var expression = GetPropertyValueAttributesAssignmentExpression(parameterVariable,
                        attribute);
                    expressions.AddIfNotNull(expression);
                }
            }

            return expressions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal static Expression GetPropertyValueAttributesAssignmentExpression(
            ParameterExpression parameterVariableExpression,
            PropertyValueAttribute attribute) =>
            GetPropertyValueAttributesAssignmentExpression((Expression)parameterVariableExpression, attribute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal static Expression GetPropertyValueAttributesAssignmentExpression(
            Expression parameterExpression,
            PropertyValueAttribute attribute)
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

            var method = GetPropertyValueAttributeSetValueMethod();
            return Expression.Call(Expression.Constant(attribute), method, parameterExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetPropertyValueAttributeSetValueMethod() =>

            StaticType.PropertyValueAttribute.GetMethod("SetValue",
                BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyValueAttribute> GetPropertyValueAttributes(ClassProperty classProperty) =>
            classProperty?
                .PropertyInfo?
                .GetCustomAttributes()?
                .Where(e =>
                    StaticType.PropertyValueAttribute.IsAssignableFrom(e.GetType()))
                .Select(e =>
                    (PropertyValueAttribute)e);

        #endregion
    }
}
