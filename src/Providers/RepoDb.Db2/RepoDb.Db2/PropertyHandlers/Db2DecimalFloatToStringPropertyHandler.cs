#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.DB2Types;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.PropertyHandlers.Db2
{
    /// <summary>
    /// A property handler that maps the Db2 <c>DECFLOAT</c> type into a <see cref="string"/> property, which keeps the full precision (up to 34 digits) and the special values (<c>NaN</c> and infinity) of the IEEE 754 decimal floating-point value.
    /// </summary>
    public class Db2DecimalFloatToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>DECFLOAT</c> value, as returned by the driver, into its text form.
        /// </summary>
        /// <param name="input">The <c>DECFLOAT</c> value of the column, as returned by the driver (a <see cref="DB2DecimalFloat"/> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The text form of the value, or <c>null</c> when the value is <c>null</c>.</returns>
        public string Get(object input,
            PropertyHandlerGetOptions options)
        {
            var value = Db2DecimalFloatConverter.ToDecimalFloat(input);
            return value.IsNull ? null : value.ToString();
        }

        /// <summary>
        /// Converts the text form of a <c>DECFLOAT</c> value into a <see cref="DB2DecimalFloat"/>, to be written into the <c>DECFLOAT</c> column.
        /// </summary>
        /// <param name="input">The text form of the value to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="DB2DecimalFloat"/> (boxed), or <c>null</c> when the text is <c>null</c> or empty.</returns>
        public object Set(string input,
            PropertyHandlerSetOptions options)
        {
            var value = Db2DecimalFloatConverter.ToDecimalFloat(input);
            return value.IsNull ? null : (object)value;
        }
    }
}
