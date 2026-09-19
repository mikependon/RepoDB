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
    /// A property handler that maps the Db2 <c>DECFLOAT</c> type into a <see cref="DB2DecimalFloat"/> property. The IEEE 754 decimal floating-point values of <c>DECFLOAT</c> (up to 34 digits, including <c>NaN</c> and infinity) are not always representable by <see cref="decimal"/>, whereas <see cref="DB2DecimalFloat"/> keeps them.
    /// </summary>
    public class Db2DecimalFloatPropertyHandler : IPropertyHandler<object, DB2DecimalFloat>
    {
        /// <summary>
        /// Converts the <c>DECFLOAT</c> value, as returned by the driver, into a <see cref="DB2DecimalFloat"/>.
        /// </summary>
        /// <param name="input">The <c>DECFLOAT</c> value of the column, as returned by the driver (a <see cref="DB2DecimalFloat"/> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="DB2DecimalFloat"/>, or <see cref="DB2DecimalFloat.Null"/> when the value is <c>null</c>.</returns>
        public DB2DecimalFloat Get(object input,
            PropertyHandlerGetOptions options) =>
            Db2DecimalFloatConverter.ToDecimalFloat(input);

        /// <summary>
        /// Converts the <see cref="DB2DecimalFloat"/> into the value written into the <c>DECFLOAT</c> column.
        /// </summary>
        /// <param name="input">The <see cref="DB2DecimalFloat"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="DB2DecimalFloat"/> (boxed), or <c>null</c> when its <see cref="DB2DecimalFloat.IsNull"/> is <c>true</c>.</returns>
        public object Set(DB2DecimalFloat input,
            PropertyHandlerSetOptions options) =>
            input.IsNull ? null : (object)input;
    }
}
