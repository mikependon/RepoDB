#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Interfaces;
using RepoDb.Models;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepoDb.Helpers
{
    /// <summary>
    /// Finds the access to a value inside a JSON column in a query expression, either via
    /// <c>ExtractValue</c> or via the <c>Value</c> property of the <see cref="DbJsonValue{T}"/>.
    /// </summary>
    public static class JsonExpressionParser
    {
        #region Private Methods

        private static JsonAccess FromExtractValue(MethodCallExpression call)
        {
            var column = Unwrap(call.Arguments[0]) as MemberExpression;
            var segments = GetExtractSegments(Unwrap(call.Arguments[1]));
            return IsEntityColumn(column) && segments != null ? new JsonAccess(column, segments) : null;
        }

        private static JsonAccess FromValueChain(MemberExpression member)
        {
            var segments = new List<string>();
            var column = TryBuildChain(member, segments);
            return IsEntityColumn(column) && segments.Count > 0 ? new JsonAccess(column, segments) : null;
        }

        private static IReadOnlyList<string> GetExtractSegments(Expression path) =>
            path switch
            {
                LambdaExpression lambda => JsonExpressionPath.FromLambda(lambda),
                ConstantExpression { Value: string text } => JsonPathBuilder.Split(text),
                _ => null
            };

        private static bool IsEntityColumn(MemberExpression column) =>
            column?.Expression is ParameterExpression;

        private static bool IsExtractValue(MethodCallExpression call) =>
            call.Method.DeclaringType == typeof(JsonQueryExtensions) &&
            string.Equals(call.Method.Name, nameof(JsonQueryExtensions.ExtractValue), System.StringComparison.Ordinal);

        private static bool IsJsonValueMember(MemberExpression member) =>
            string.Equals(member.Member.Name, nameof(DbJsonValue<object>.Value), System.StringComparison.Ordinal) &&
            member.Expression is MemberExpression &&
            typeof(IDbJsonValue).IsAssignableFrom(member.Expression.Type);

        private static MemberExpression TryBuildChain(Expression expression,
            List<string> segments)
        {
            switch (Unwrap(expression))
            {
                case MemberExpression member when IsJsonValueMember(member):
                    return (MemberExpression)member.Expression;
                case MemberExpression member:
                    return AppendAfter(TryBuildChain(member.Expression, segments), segments, JsonExpressionPath.GetName(member.Member));
                case BinaryExpression binary when binary.NodeType == ExpressionType.ArrayIndex:
                    return AppendAfter(TryBuildChain(binary.Left, segments), segments, JsonExpressionPath.GetIndex(binary.Right));
                case MethodCallExpression call when call.Method.Name == "get_Item" && call.Arguments.Count == 1:
                    return AppendAfter(TryBuildChain(call.Object, segments), segments, JsonExpressionPath.GetIndex(call.Arguments[0]));
                default:
                    return null;
            }
        }

        private static MemberExpression AppendAfter(MemberExpression column,
            List<string> segments,
            string segment)
        {
            if (column != null)
            {
                segments.Add(segment);
            }
            return column;
        }

        private static Expression Unwrap(Expression expression)
        {
            while (expression is UnaryExpression unary &&
                (unary.NodeType == ExpressionType.Convert || unary.NodeType == ExpressionType.ConvertChecked || unary.NodeType == ExpressionType.Quote))
            {
                expression = unary.Operand;
            }
            return expression;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tries to find the access to a value inside a JSON column in the expression.
        /// </summary>
        /// <param name="expression">The expression, usually the left side of a comparison.</param>
        /// <param name="access">The found access, or <c>null</c> when the expression is not a JSON access.</param>
        /// <returns><c>true</c> if the expression is a JSON access.</returns>
        public static bool TryParse(Expression expression,
            out JsonAccess access)
        {
            access = Unwrap(expression) switch
            {
                MethodCallExpression call when IsExtractValue(call) => FromExtractValue(call),
                MemberExpression member => FromValueChain(member),
                _ => null
            };
            return access != null;
        }

        #endregion
    }
}

#endif
