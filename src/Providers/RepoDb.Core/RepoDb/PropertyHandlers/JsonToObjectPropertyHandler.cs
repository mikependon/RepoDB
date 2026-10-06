#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>json</c> type into a <typeparamref name="TObject"/> property, via <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="TObject">The type of the property.</typeparam>
    public class JsonToObjectPropertyHandler<TObject> : IPropertyHandler<string, TObject>
    {
        private const string JsonReflectionMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed " +
            "and might need runtime code generation. Use the constructor that accepts a 'JsonTypeInfo<TObject>' (System.Text.Json source generation) instead.";

        private readonly JsonTypeInfo<TObject> jsonTypeInfo;

        /// <summary>
        /// Creates a new instance of <see cref="JsonToObjectPropertyHandler{TObject}"/> class that uses the reflection-based serialization of <see cref="JsonSerializer"/>.
        /// </summary>
        [RequiresUnreferencedCode(JsonReflectionMessage)]
        [RequiresDynamicCode(JsonReflectionMessage)]
        public JsonToObjectPropertyHandler()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonToObjectPropertyHandler{TObject}"/> class that uses the given (i.e.: source generated) JSON type information,
        /// which is compatible with the trimming and NativeAOT.
        /// </summary>
        /// <param name="jsonTypeInfo">The JSON type information of <typeparamref name="TObject"/>.</param>
        public JsonToObjectPropertyHandler(JsonTypeInfo<TObject> jsonTypeInfo)
        {
            this.jsonTypeInfo = jsonTypeInfo ?? throw new System.ArgumentNullException(nameof(jsonTypeInfo));
        }

        /// <summary>
        /// Deserializes the SQL Server <c>json</c> value, as returned by the database, into a <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The deserialized <typeparamref name="TObject"/>, or its default value when the value is <c>null</c> or empty.</returns>
        public TObject Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? default : Deserialize(input);

        /// <summary>
        /// Serializes the <typeparamref name="TObject"/> into JSON text, to be written into a SQL Server <c>json</c> column.
        /// </summary>
        /// <param name="input">The <typeparamref name="TObject"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The JSON text, or <c>null</c> when the <typeparamref name="TObject"/> is <c>null</c>.</returns>
        public string Set(TObject input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : Serialize(input);

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "The reflection-based serialization is only used when created via the constructor annotated with RequiresUnreferencedCode.")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "The reflection-based serialization is only used when created via the constructor annotated with RequiresDynamicCode.")]
        private TObject Deserialize(string input) =>
            jsonTypeInfo != null ? JsonSerializer.Deserialize(input, jsonTypeInfo) : JsonSerializer.Deserialize<TObject>(input);

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "The reflection-based serialization is only used when created via the constructor annotated with RequiresUnreferencedCode.")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "The reflection-based serialization is only used when created via the constructor annotated with RequiresDynamicCode.")]
        private string Serialize(TObject input) =>
            jsonTypeInfo != null ? JsonSerializer.Serialize(input, jsonTypeInfo) : JsonSerializer.Serialize(input);
    }
}

#endif
