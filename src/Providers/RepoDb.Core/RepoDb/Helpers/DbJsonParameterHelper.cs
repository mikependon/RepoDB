#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Helpers
{
    /// <summary>
    /// Prepares the command parameters that carry a <see cref="IDbJsonValue"/>.
    /// </summary>
    public static class DbJsonParameterHelper
    {
        #region Public Methods

        /// <summary>
        /// Configures the parameter that carries the JSON text, using the setting of the database.
        /// </summary>
        /// <param name="parameter">The parameter to configure.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        public static void Configure(IDbDataParameter parameter,
            IDbSetting dbSetting)
        {
            DbJsonDialectMapper.Get(dbSetting).ConfigureParameter(parameter);
        }

        /// <summary>
        /// Converts a <see cref="IDbJsonValue"/> into its JSON text. Any other value is returned as is.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="converted">The JSON text (or <c>null</c> when the value is empty), or the original value when not a JSON value.</param>
        /// <returns><c>true</c> if the value is a <see cref="IDbJsonValue"/>.</returns>
        public static bool TryConvert(object value,
            out object converted)
        {
            if (value is IDbJsonValue json)
            {
                converted = json.ToJsonString();
                return true;
            }
            converted = value;
            return false;
        }

        #endregion
    }
}
