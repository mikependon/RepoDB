#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A property handler that maps the Oracle <c>VECTOR</c> type into a <see cref="double"/> array property.
    /// </summary>
    public class OracleVectorToDoubleArrayPropertyHandler : IPropertyHandler<object, double[]>
    {
        /// <summary>
        /// Converts the Oracle <c>VECTOR</c> value, as returned by the driver, into a <see cref="double"/> array.
        /// </summary>
        /// <param name="input">The <c>VECTOR</c> value of the column, as returned by the driver (an <see cref="OracleVector"/>, a numeric array, or a JSON array <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="double"/> array with the elements of the vector, or <c>null</c> when the value is <c>null</c>.</returns>
        public double[] Get(object input,
            PropertyHandlerGetOptions options) =>
            OracleVectorConverter.ToDoubleArray(input);

        /// <summary>
        /// Converts the <see cref="double"/> array into an <see cref="OracleVector"/> (<c>FLOAT64</c>), to be written into an Oracle <c>VECTOR</c> column.
        /// </summary>
        /// <param name="input">The <see cref="double"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="OracleVector"/> (boxed), or <c>null</c> when the array is <c>null</c>.</returns>
        public object Set(double[] input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : new OracleVector(input);
    }
}
