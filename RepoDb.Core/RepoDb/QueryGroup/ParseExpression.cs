using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb
{
    public partial class QueryGroup
    {
        /*
         * Others
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool IsDirect(BinaryExpression expression) =>
            (
                expression.Left.NodeType == ExpressionType.Constant ||
                expression.Left.NodeType == ExpressionType.Convert ||
                expression.Left.NodeType == ExpressionType.MemberAccess
            )
            &&
            (
                expression.Right.NodeType == ExpressionType.Call ||
                expression.Right.NodeType == ExpressionType.Conditional ||
                expression.Right.NodeType == ExpressionType.Constant ||
                expression.Right.NodeType == ExpressionType.Convert ||
                expression.Right.NodeType == ExpressionType.MemberAccess ||
                expression.Right.NodeType == ExpressionType.NewArrayInit
            );

        /*
         * Expression
         */

        /// <summary>
        /// Parses a customized query expression.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type</typeparam>
        /// <param name="expression">The expression to be converted to a <see cref="QueryGroup"/> object.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> object that contains the parsed query expression.</returns>
        public static QueryGroup Parse<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            // Guard the presence of the expression
            if (expression == null)
            {
                throw new NullReferenceException("Expression cannot be null.");
            }

            // Parse the expression base on type
            var parsed = Parse<TEntity>(expression.Body);

            /*
             * In order to NOT trigger the 'Equality' comparision (via overriden GetHashCode()), do not use the '=='
             * when comparing to NULLs, instead, use the ReferenceEquals method.
             */

            // Throw an unsupported exception if not parsed
            if (ReferenceEquals(parsed, null))
            {
                throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
            }

            // Return the parsed values
            return parsed.Fix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(Expression expression)
            where TEntity : class
        {
            if (expression.IsLambda())
            {
                return Parse<TEntity>(expression.ToLambda().Body);
            }
            else if (expression.IsBinary())
            {
                return Parse<TEntity>(expression.ToBinary());
            }
            else if (expression.IsUnary())
            {
                return Parse<TEntity>(expression.ToUnary());
            }
            else if (expression.IsMethodCall())
            {
                return Parse<TEntity>(expression.ToMethodCall());
            }
            return null;
        }

        /*
         * Binary
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(BinaryExpression expression)
            where TEntity : class
        {
            // Check directness
            if (IsDirect(expression))
            {
                return new QueryGroup(QueryField.Parse<TEntity>(expression));
            }

            // Variables
            var leftQueryGroup = Parse<TEntity>(expression.Left);

            // IsNot
            if (expression.Right.Type == StaticType.Boolean && expression.IsExtractable() == true)
            {
                var rightValue = (bool)expression.Right.GetValue();
                var isNot = (expression.NodeType == ExpressionType.Equal && rightValue == false) ||
                    (expression.NodeType == ExpressionType.NotEqual && rightValue == true);
                leftQueryGroup?.SetIsNot(isNot);
            }
            else
            {
                var rightQueryGroup = Parse<TEntity>(expression.Right);
                if (rightQueryGroup != null)
                {
                    return new QueryGroup(new[] { leftQueryGroup, rightQueryGroup }, GetConjunction(expression));
                }
            }

            // Return
            return leftQueryGroup;
        }

        /*
         * Unary
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(UnaryExpression expression)
            where TEntity : class
        {
            var queryGroup = (QueryGroup)null;

            if (expression.Operand.IsMember() == true)
            {
                queryGroup = Parse<TEntity>(expression.Operand.ToMember(), expression.NodeType);
            }
            else if (expression.Operand.IsMethodCall() == true)
            {
                queryGroup = Parse<TEntity>(expression.Operand.ToMethodCall(), expression.NodeType);
            }

            return queryGroup;
        }

        /*
         * Member
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MemberExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
            return queryFields != null ? new QueryGroup(queryFields) : null;
        }

        /*
         * MethodCall
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MethodCallExpression expression)
            where TEntity : class
        {
            var unaryNodeType = (expression?.Object?.Type == StaticType.String) ? GetNodeType(expression.Object.ToMember()) :
                GetNodeType(expression.Arguments.LastOrDefault());
            return Parse<TEntity>(expression, unaryNodeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MethodCallExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            var queryFields = QueryField.Parse<TEntity>(expression, unaryNodeType);
            return queryFields != null ? new QueryGroup(queryFields, GetConjunction(expression)) : null;
        }

        #region GetConjunction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Conjunction GetConjunction(BinaryExpression expression) =>
            expression.NodeType == ExpressionType.Or || expression.NodeType == ExpressionType.OrElse ?
            Conjunction.Or : Conjunction.And;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Conjunction GetConjunction(MethodCallExpression expression) =>
            expression.Method.Name == "Any" ? Conjunction.Or : Conjunction.And;

        #endregion

        #region GetNodeType

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ExpressionType? GetNodeType(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            if (expression.IsLambda())
            {
                return GetNodeType(expression.ToLambda());
            }
            else if (expression.IsBinary())
            {
                return GetNodeType(expression.ToBinary());
            }
            else if (expression.IsMethodCall())
            {
                return GetNodeType(expression.ToMethodCall());
            }
            else if (expression.IsMember())
            {
                return GetNodeType(expression.ToMember());
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ExpressionType? GetNodeType(LambdaExpression expression) =>
            GetNodeType(expression.ToLambda().Body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ExpressionType? GetNodeType(BinaryExpression expression) =>
            expression.NodeType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ExpressionType? GetNodeType(MemberExpression expression) =>
            expression.NodeType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ExpressionType? GetNodeType(MethodCallExpression expression) =>
            expression.NodeType;

        #endregion
    }
}
