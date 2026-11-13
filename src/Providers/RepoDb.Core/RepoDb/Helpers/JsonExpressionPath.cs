#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace RepoDb.Helpers
{
    /// <summary>
    /// Converts the member and array index accessors of an expression (i.e.: <c>d =&gt; d.Address.City</c>) into JSON path segments.
    /// </summary>
    public static class JsonExpressionPath
    {
        #region Private Methods

        /// <summary>
        /// Adds the segments of a member access.
        /// </summary>
        /// <param name="expression">The expression to inspect.</param>
        /// <param name="segments">The collected segments.</param>
        private static void AddMember(MemberExpression expression,
            List<string> segments)
        {
            Collect(expression.Expression, segments);
            segments.Add(GetName(expression.Member));
        }

        /// <summary>
        /// Adds the segments of the accessor chain.
        /// </summary>
        /// <param name="expression">The expression to inspect.</param>
        /// <param name="segments">The collected segments.</param>
        private static void Collect(Expression expression,
            List<string> segments)
        {
            switch (expression)
            {
                case MemberExpression member:
                    AddMember(member, segments);
                    break;
                case BinaryExpression binary when binary.NodeType == ExpressionType.ArrayIndex:
                    Collect(binary.Left, segments);
                    segments.Add(GetIndex(binary.Right));
                    break;
                case MethodCallExpression call when call.Method.Name == "get_Item" && call.Arguments.Count == 1:
                    Collect(call.Object, segments);
                    segments.Add(GetIndex(call.Arguments[0]));
                    break;
                case UnaryExpression unary when unary.NodeType == ExpressionType.Convert:
                    Collect(unary.Operand, segments);
                    break;
                case ParameterExpression:
                    break;
                default:
                    throw new NotSupportedException($"Expression '{expression}' is not supported as a JSON path.");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the JSON path segment of an array index expression.
        /// </summary>
        /// <param name="indexExpression">The expression of the index (a constant or a captured value).</param>
        /// <returns>The index in brackets (i.e.: <c>[0]</c>).</returns>
        public static string GetIndex(Expression indexExpression)
        {
            var index = Convert.ToInt32(indexExpression.GetValue(), CultureInfo.InvariantCulture);
            return string.Concat("[", index.ToString(CultureInfo.InvariantCulture), "]");
        }

        /// <summary>
        /// Gets the JSON property name of a member, using its <see cref="JsonPropertyNameAttribute"/> or the naming policy of the options.
        /// </summary>
        /// <param name="member">The member of the object the JSON represents.</param>
        /// <returns>The JSON property name.</returns>
        public static string GetName(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attribute != null)
            {
                return attribute.Name;
            }
            var policy = DbJsonConverter.GetOptions().PropertyNamingPolicy;
            return policy == null ? member.Name : policy.ConvertName(member.Name);
        }

        /// <summary>
        /// Gets the JSON path segments of the body of a lambda expression.
        /// </summary>
        /// <param name="expression">The lambda expression (i.e.: <c>d =&gt; d.Address.City</c>).</param>
        /// <returns>The property and array index segments.</returns>
        public static IReadOnlyList<string> FromLambda(LambdaExpression expression)
        {
            var segments = new List<string>();
            Collect(expression.Body, segments);
            return segments;
        }

        #endregion
    }
}

#endif
