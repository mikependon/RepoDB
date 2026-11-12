#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace RepoDb
{
    /// <summary>
    /// Extensions to extract a value from a JSON column. In a query expression, the call is translated into the JSON function of the database.
    /// </summary>
    public static class JsonQueryExtensions
    {
        #region Private Methods

        private static TResult Extract<TResult>(JsonNode source,
            IReadOnlyList<string> segments) =>
            Navigate(source, segments) is { } node ? node.GetValue<TResult>() : default;

        private static JsonNode Navigate(JsonNode node,
            IReadOnlyList<string> segments)
        {
            foreach (var segment in segments)
            {
                if (node == null)
                {
                    return null;
                }
                node = Step(node, segment);
            }
            return node;
        }

        private static JsonNode Step(JsonNode node,
            string segment) =>
            JsonPathBuilder.IsIndex(segment) ?
                StepIndex(node, segment) :
                (node is JsonObject jsonObject ? jsonObject[segment] : null);

        private static JsonNode StepIndex(JsonNode node,
            string segment)
        {
            var index = int.Parse(segment.AsSpan(1, segment.Length - 2), provider: System.Globalization.CultureInfo.InvariantCulture);
            return node is JsonArray jsonArray && index < jsonArray.Count ? jsonArray[index] : null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts a value from the JSON node using a path (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).
        /// </summary>
        /// <typeparam name="TResult">The type of the extracted value.</typeparam>
        /// <param name="source">The JSON node.</param>
        /// <param name="path">The path of the value.</param>
        /// <returns>The extracted value, or the default value when the path is not found.</returns>
        public static TResult ExtractValue<TResult>(this JsonNode source,
            string path) =>
            Extract<TResult>(source, JsonPathBuilder.Split(path));

        /// <summary>
        /// Extracts a value from the <see cref="DbJsonValue{TEntity}"/> using a path (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the object the JSON represents.</typeparam>
        /// <typeparam name="TResult">The type of the extracted value.</typeparam>
        /// <param name="source">The JSON value.</param>
        /// <param name="path">The path of the value.</param>
        /// <returns>The extracted value, or the default value when the path is not found.</returns>
        public static TResult ExtractValue<TEntity, TResult>(this DbJsonValue<TEntity> source,
            string path)
            where TEntity : class =>
            Extract<TResult>(source.Json, JsonPathBuilder.Split(path));

        /// <summary>
        /// Extracts a value from the <see cref="DbJsonValue{TEntity}"/> using a member expression (i.e.: <c>d =&gt; d.Address.City</c>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the object the JSON represents.</typeparam>
        /// <typeparam name="TResult">The type of the extracted value.</typeparam>
        /// <param name="source">The JSON value.</param>
        /// <param name="mapping">The member expression of the value.</param>
        /// <returns>The extracted value, or the default value when the path is not found.</returns>
        public static TResult ExtractValue<TEntity, TResult>(this DbJsonValue<TEntity> source,
            Expression<Func<TEntity, TResult>> mapping)
            where TEntity : class =>
            Extract<TResult>(source.Json, JsonExpressionPath.FromLambda(mapping));

        #endregion
    }
}

#endif
