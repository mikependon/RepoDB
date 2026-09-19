#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlTypes;
using RepoDb.Interfaces;
using RepoDb.Options;
using System.Globalization;
using System.Linq;

namespace RepoDb.PropertyHandlers.SqlServer
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>vector</c> type into a <see cref="float"/> array property.
    /// </summary>
    public class SqlServerVectorToFloatArrayPropertyHandler : IPropertyHandler<object, float[]>
    {
        /// <summary>
        /// Converts the SQL Server <c>vector</c> value, as returned by the database, into a <see cref="float"/> array.
        /// </summary>
        /// <param name="input">The <c>vector</c> value of the column, as returned by the driver (a <see cref="SqlVector{T}"/> of <see cref="float"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="float"/> array with the elements of the vector, or <c>null</c> when the value is <c>null</c>.</returns>
        public float[] Get(object input,
            PropertyHandlerGetOptions options) =>
            input is SqlVector<float> { IsNull: false } vector ? vector.Memory.ToArray() : null;

        /// <summary>
        /// Converts the <see cref="float"/> array into its JSON array text (for example <c>[1.5,2.5,3.5]</c>, using the invariant culture), which SQL Server converts implicitly when written into a <c>vector</c> column.
        /// </summary>
        /// <param name="input">The <see cref="float"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The JSON array text, or <c>null</c> when the array is <c>null</c>.</returns>
        public object Set(float[] input,
            PropertyHandlerSetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            return "[" + string.Join(",",
                input.Select(value => value.ToString("R",
                    CultureInfo.InvariantCulture))) + "]";
        }
    }
}
