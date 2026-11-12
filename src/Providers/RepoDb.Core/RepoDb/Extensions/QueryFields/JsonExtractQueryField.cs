#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations;
using RepoDb.Helpers;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A functional-based <see cref="QueryField"/> that compares a value extracted from a JSON column, using the JSON function of the database.
    /// </summary>
    public sealed class JsonExtractQueryField : FunctionalQueryField
    {
        private const string DefaultFormat = "JSON_EXTRACT({0}, " + JsonConstant.PathPlaceholder + ")";

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="JsonExtractQueryField"/> class with the equal operation.
        /// </summary>
        /// <param name="fieldName">The name of the JSON column.</param>
        /// <param name="path">The path of the value inside the JSON (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).</param>
        /// <param name="value">The value to compare with.</param>
        public JsonExtractQueryField(string fieldName,
            string path,
            object value)
            : this(fieldName, path, Operation.Equal, value, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonExtractQueryField"/> class.
        /// </summary>
        /// <param name="fieldName">The name of the JSON column.</param>
        /// <param name="path">The path of the value inside the JSON (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).</param>
        /// <param name="operation">The operation of the comparison.</param>
        /// <param name="value">The value to compare with.</param>
        /// <param name="dbType">The database type of the value.</param>
        public JsonExtractQueryField(string fieldName,
            string path,
            Operation operation,
            object value,
            DbType? dbType)
            : base(fieldName, operation, value, dbType, DefaultFormat)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("The JSON path is required.", nameof(path));
            }
            Path = path;
            Segments = JsonPathBuilder.Split(path);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path of the value inside the JSON.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the property and array index segments of the path.
        /// </summary>
        public IReadOnlyList<string> Segments { get; }

        #endregion

        #region Private Methods

        private string GetFormat(IDbSetting dbSetting) =>
            DbJsonDialectMapper.Get(dbSetting).GetExtractFormat(Parameter.Value?.GetType());

        private string GetPath(IDbSetting dbSetting) =>
            DbJsonDialectMapper.Get(dbSetting).FormatPath(Segments);

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the string representation of the query field, where the column is replaced by the JSON extraction.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representation of the query field.</returns>
        public override string GetString(int index,
            IDbSetting dbSetting)
        {
            var text = GetString(index, GetFormat(dbSetting), dbSetting);
            return text.Replace(JsonConstant.PathPlaceholder, GetPath(dbSetting));
        }

        /// <summary>
        /// Gets the hash code of the query field, including its path.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), StringComparer.Ordinal.GetHashCode(Path));

        #endregion
    }
}
