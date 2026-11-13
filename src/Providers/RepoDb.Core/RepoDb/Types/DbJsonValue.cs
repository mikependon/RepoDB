#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Helpers;
using RepoDb.Interfaces;
using System;
using System.Text.Json.Nodes;

namespace RepoDb
{
    /// <summary>
    /// A value stored in a JSON column. RepoDb converts it from and to its JSON text.
    /// </summary>
    /// <typeparam name="T">The type of the object the JSON represents.</typeparam>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public readonly struct DbJsonValue<T> : IDbJsonValue, IEquatable<DbJsonValue<T>>
        where T : class
    {
        private readonly JsonNode json;
        private readonly T value;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="DbJsonValue{T}"/> from a JSON node.
        /// </summary>
        /// <param name="json">The JSON node.</param>
        public DbJsonValue(JsonNode json)
        {
            ArgumentNullException.ThrowIfNull(json);
            this.json = json;
            value = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbJsonValue{T}"/> from an object.
        /// </summary>
        /// <param name="value">The object the JSON represents.</param>
        public DbJsonValue(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            json = null;
            this.value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the value has no JSON content.
        /// </summary>
        public bool IsEmpty => json == null && value == null;

        /// <summary>
        /// Gets the JSON node of the value.
        /// </summary>
        public JsonNode Json => json ?? DbJsonConverter.ToNode(value);

        /// <summary>
        /// Gets the object the JSON represents.
        /// </summary>
        public T Value => value ?? DbJsonConverter.FromNode<T>(json);

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares the value with another value.
        /// </summary>
        /// <param name="other">The other value.</param>
        /// <returns><c>true</c> if both values have the same JSON content.</returns>
        public bool Equals(DbJsonValue<T> other) =>
            JsonNode.DeepEquals(Json, other.Json);

        /// <summary>
        /// Compares the value with an object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><c>true</c> if the object is a <see cref="DbJsonValue{T}"/> with the same JSON content.</returns>
        public override bool Equals(object obj) =>
            obj is DbJsonValue<T> other && Equals(other);

        /// <summary>
        /// Gets the hash code of the value.
        /// </summary>
        /// <returns>The hash code, based on the kind of the JSON content.</returns>
        public override int GetHashCode() =>
            Json?.GetValueKind().GetHashCode() ?? 0;

        /// <summary>
        /// Gets the JSON text of the value.
        /// </summary>
        /// <returns>The JSON text, or <c>null</c> when the value is empty.</returns>
        public string ToJsonString() =>
            IsEmpty ? null : DbJsonConverter.ToJsonString(Json);

        /// <summary>
        /// Gets the JSON text of the value.
        /// </summary>
        /// <returns>The JSON text, or <c>null</c> (as text) when the value is empty.</returns>
        public override string ToString() =>
            ToJsonString() ?? "null";

        /// <summary>
        /// Parses the JSON text into a <see cref="DbJsonValue{T}"/>.
        /// </summary>
        /// <param name="json">The JSON text.</param>
        /// <returns>The value, or an empty value when the text is null or empty.</returns>
        public static DbJsonValue<T> Parse(string json)
        {
            var node = DbJsonConverter.Parse(json);
            return node == null ? default : new DbJsonValue<T>(node);
        }

        /// <summary>
        /// Tries to parse the JSON text into a <see cref="DbJsonValue{T}"/>.
        /// </summary>
        /// <param name="json">The JSON text.</param>
        /// <param name="result">The parsed value, or an empty value when the parsing failed.</param>
        /// <returns><c>true</c> if the text is a valid JSON.</returns>
        public static bool TryParse(string json,
            out DbJsonValue<T> result)
        {
            try
            {
                result = Parse(json);
                return !result.IsEmpty;
            }
            catch (System.Text.Json.JsonException)
            {
                result = default;
                return false;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Converts an object into a <see cref="DbJsonValue{T}"/>.
        /// </summary>
        /// <param name="value">The object the JSON represents.</param>
        public static implicit operator DbJsonValue<T>(T value) =>
            value == null ? default : new DbJsonValue<T>(value);

        /// <summary>
        /// Converts the JSON text into a <see cref="DbJsonValue{T}"/>.
        /// </summary>
        /// <param name="json">The JSON text.</param>
        public static explicit operator DbJsonValue<T>(string json) =>
            Parse(json);

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><c>true</c> if both values have the same JSON content.</returns>
        public static bool operator ==(DbJsonValue<T> left,
            DbJsonValue<T> right) =>
            left.Equals(right);

        /// <summary>
        /// Compares two values for inequality.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><c>true</c> if the values have different JSON content.</returns>
        public static bool operator !=(DbJsonValue<T> left,
            DbJsonValue<T> right) =>
            !left.Equals(right);

        #endregion
    }
#pragma warning restore CA1000
}

#endif
