#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A property handler that maps the Firebird <c>DECFLOAT(16)</c> and <c>DECFLOAT(34)</c> types into a <see cref="string"/> property, which keeps the full precision and the special values (<c>NaN</c> and infinity) of the IEEE 754 decimal floating-point value.
    /// </summary>
    public class FirebirdDecFloatToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>DECFLOAT</c> value, as returned by the driver, into its text form.
        /// </summary>
        /// <param name="input">The <c>DECFLOAT</c> value of the column, as returned by the driver (an <c>FbDecFloat</c> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The text form of the value, or <c>null</c> when the value is <c>null</c>.</returns>
        public string Get(object input,
            PropertyHandlerGetOptions options) =>
            FirebirdDecFloatConverter.ToText(input);

        /// <summary>
        /// Passes the text form of the value to the driver, which Firebird converts to <c>DECFLOAT</c> on assignment.
        /// </summary>
        /// <param name="input">The text form of the <c>DECFLOAT</c> value to write (for example <c>123.45</c> or <c>1.5E+20</c>).</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The text itself, or <c>null</c> when the text is <c>null</c>.</returns>
        public object Set(string input,
            PropertyHandlerSetOptions options) =>
            input;
    }
}
