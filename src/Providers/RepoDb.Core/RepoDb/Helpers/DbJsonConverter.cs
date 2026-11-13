#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace RepoDb.Helpers
{
    /// <summary>
    /// Converts the values from and to JSON using the <see cref="JsonSerializerOptions"/> of the <see cref="GlobalConfiguration"/>.
    /// </summary>
    public static class DbJsonConverter
    {
        #region Private Methods

        /// <summary>
        /// Gets the JSON type information of the type from the configured options.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The JSON type information.</returns>
        private static JsonTypeInfo GetTypeInfo(Type type) =>
            GetOptions().GetTypeInfo(type);

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the JSON node into a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="node">The JSON node.</param>
        /// <returns>The deserialized value, or <c>null</c> when the node is <c>null</c>.</returns>
        public static T FromNode<T>(JsonNode node)
            where T : class =>
            node == null ? null : (T)JsonSerializer.Deserialize(node, GetTypeInfo(typeof(T)));

        /// <summary>
        /// Gets the options used to serialize and deserialize the JSON values.
        /// </summary>
        /// <returns>The options of the <see cref="GlobalConfiguration"/>, or the default options when not set.</returns>
        public static JsonSerializerOptions GetOptions() =>
            GlobalConfiguration.Options.JsonSerializerOptions ?? JsonSerializerOptions.Default;

        /// <summary>
        /// Parses the JSON text into a JSON node.
        /// </summary>
        /// <param name="json">The JSON text.</param>
        /// <returns>The JSON node, or <c>null</c> when the text is null or empty.</returns>
        public static JsonNode Parse(string json) =>
            string.IsNullOrWhiteSpace(json) ? null : JsonNode.Parse(json);

        /// <summary>
        /// Serializes the value into a JSON node.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON node, or <c>null</c> when the value is <c>null</c>.</returns>
        public static JsonNode ToNode<T>(T value)
            where T : class =>
            value == null ? null : JsonSerializer.SerializeToNode(value, GetTypeInfo(typeof(T)));

        /// <summary>
        /// Gets the JSON text of the JSON node.
        /// </summary>
        /// <param name="node">The JSON node.</param>
        /// <returns>The JSON text, or <c>null</c> when the node is <c>null</c>.</returns>
        public static string ToJsonString(JsonNode node) =>
            node?.ToJsonString(GetOptions());

        #endregion
    }
}

#endif
