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
    /// Contains the extension methods for <see cref="Order"/>.
    /// </summary>
    public static class OrderExtension
    {
        /// <summary>
        /// Gets the text value is used to defined the <see cref="Order"/>.
        /// </summary>
        public static string GetText(this Order order) => order switch
        {
            Order.Ascending => "ASC",
            Order.Descending => "DESC",
            _ => throw new ArgumentOutOfRangeException(nameof(order))
        };
    }
}