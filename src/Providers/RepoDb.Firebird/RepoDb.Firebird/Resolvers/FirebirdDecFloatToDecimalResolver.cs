#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using RepoDb.Interfaces;
using System;
using System.Globalization;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the value of a Firebird <c>DECFLOAT</c> column, as returned by the driver
    /// (an <see cref="FbDecFloat"/>, a <see cref="decimal"/> or a <see cref="string"/>), into a <see cref="decimal"/>.
    /// </summary>
    public class FirebirdDecFloatToDecimalResolver : IResolver<object, decimal?>
    {
        private readonly IResolver<object, string> textResolver;

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDecFloatToDecimalResolver"/> class.
        /// </summary>
        public FirebirdDecFloatToDecimalResolver()
            : this(new FirebirdDecFloatToTextResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDecFloatToDecimalResolver"/> class.
        /// </summary>
        /// <param name="textResolver">The resolver that is used to convert the value returned by the driver into text.</param>
        public FirebirdDecFloatToDecimalResolver(IResolver<object, string> textResolver)
        {
            this.textResolver = textResolver;
        }

        /// <summary>
        /// Converts the value returned by the driver (an <see cref="FbDecFloat"/>, a <see cref="decimal"/> or a <see cref="string"/>) into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="decimal"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of a <see cref="decimal"/>.</exception>
        /// <exception cref="FormatException">Thrown when the value is not a finite number (<c>NaN</c> or infinity).</exception>
        public virtual decimal? Resolve(object value)
        {
            if (value is decimal number)
            {
                return number;
            }
            var text = textResolver.Resolve(value);
            return text == null ? (decimal?)null : decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}
