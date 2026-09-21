#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using RepoDb.Interfaces;
using System;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the text form of a Firebird <c>DECFLOAT</c> value into the <see cref="FbDecFloat"/> that is written into the column.
    /// </summary>
    public class TextToFirebirdDecFloatResolver : IResolver<string, object>
    {
        /// <summary>
        /// Converts the text form of a finite <c>DECFLOAT</c> value (for example <c>123.45</c> or <c>1.5E+20</c>) into an <see cref="FbDecFloat"/>, keeping its coefficient and scale.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The <see cref="FbDecFloat"/> (boxed), or the text itself when it is not a finite number (for example <c>NaN</c>), to be converted by the server.</returns>
        public virtual object Resolve(string text)
        {
            var match = Regex.Match(text ?? string.Empty,
                @"^\s*([+-]?)(\d*)(?:\.(\d*))?(?:[eE]([+-]?\d+))?\s*$",
                RegexOptions.CultureInvariant);
            var digits = match.Success ? match.Groups[2].Value + match.Groups[3].Value : string.Empty;
            if (digits.Length == 0 ||
                (match.Groups[4].Success && int.TryParse(match.Groups[4].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _) == false))
            {
                return text;
            }
            var coefficient = BigInteger.Parse(digits, CultureInfo.InvariantCulture);
            if (string.Equals(match.Groups[1].Value, "-", StringComparison.Ordinal))
            {
                coefficient = -coefficient;
            }
            var exponent = match.Groups[4].Success ? int.Parse(match.Groups[4].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture) : 0;
            return new FbDecFloat(coefficient, exponent - match.Groups[3].Value.Length);
        }
    }
}
