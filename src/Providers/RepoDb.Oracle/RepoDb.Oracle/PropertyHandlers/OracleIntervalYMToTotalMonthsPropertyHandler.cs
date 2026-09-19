#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A property handler that maps the Oracle <c>INTERVAL YEAR TO MONTH</c> type into a total number of months, as a <see cref="long"/> property. A <see cref="TimeSpan"/> cannot be used for this type, as years and months are calendar units of variable length.
    /// </summary>
    public class OracleIntervalYMToTotalMonthsPropertyHandler : IPropertyHandler<object, long?>
    {
        /// <summary>
        /// Converts the <c>INTERVAL YEAR TO MONTH</c> value, as returned by the driver, into the total number of months.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (the total number of months as a <see cref="long"/>, or an <see cref="OracleIntervalYM"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The total number of months (years multiplied by 12, plus months), or <c>null</c> when the value is <c>null</c>.</returns>
        public long? Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return null;
                case OracleIntervalYM interval:
                    return interval.IsNull ? (long?)null : interval.Value;
                default:
                    return Convert.ToInt64(input);
            }
        }

        /// <summary>
        /// Converts the total number of months into an <see cref="OracleIntervalYM"/>, to be written into an <c>INTERVAL YEAR TO MONTH</c> column.
        /// </summary>
        /// <param name="input">The total number of months to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="OracleIntervalYM"/> (boxed), or <c>null</c> when the value is <c>null</c>.</returns>
        public object Set(long? input,
            PropertyHandlerSetOptions options) =>
            input.HasValue ? new OracleIntervalYM(input.Value) : null;
    }
}
