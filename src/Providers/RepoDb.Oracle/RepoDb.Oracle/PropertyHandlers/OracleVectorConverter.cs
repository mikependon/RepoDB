#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using System;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A helper that converts the value of an Oracle <c>VECTOR</c> column, as returned by the driver, into numeric arrays.
    /// </summary>
    internal static class OracleVectorConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (an <see cref="OracleVector"/>, a <see cref="float"/> array, a <see cref="double"/> array, or a JSON array <see cref="string"/> such as <c>[1.5,2.5]</c>) into a <see cref="float"/> array.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="float"/> array, or <c>null</c> when the value is <c>null</c>, <see cref="DBNull"/> or a null <see cref="OracleVector"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static float[] ToFloatArray(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case float[] singles:
                    return singles;
                case double[] doubles:
                    return Array.ConvertAll(doubles, item => (float)item);
                case OracleVector vector:
                    return vector.IsNull ? null : vector.ToFloatArray();
                case string text:
                    return text.Length == 0 ? null : new OracleVector(text).ToFloatArray();
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported vector value.", nameof(value));
            }
        }

        /// <summary>
        /// Converts the value returned by the driver (an <see cref="OracleVector"/>, a <see cref="double"/> array, a <see cref="float"/> array, or a JSON array <see cref="string"/> such as <c>[1.5,2.5]</c>) into a <see cref="double"/> array.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="double"/> array, or <c>null</c> when the value is <c>null</c>, <see cref="DBNull"/> or a null <see cref="OracleVector"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static double[] ToDoubleArray(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case double[] doubles:
                    return doubles;
                case float[] singles:
                    return Array.ConvertAll(singles, item => (double)item);
                case OracleVector vector:
                    return vector.IsNull ? null : vector.ToDoubleArray();
                case string text:
                    return text.Length == 0 ? null : new OracleVector(text).ToDoubleArray();
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported vector value.", nameof(value));
            }
        }
    }
}
