#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Conjunction"/>.
    /// </summary>
    public static class ConjunctionExtension
    {
        /// <summary>
        /// Gets the text value is used to defined the <see cref="Conjunction"/>.
        /// </summary>
        public static string GetText(this Conjunction conjunction) => conjunction switch
        {
            Conjunction.And => "AND",
            Conjunction.Or => "OR",
            _ => throw new ArgumentOutOfRangeException(nameof(conjunction))
        };
    }
}