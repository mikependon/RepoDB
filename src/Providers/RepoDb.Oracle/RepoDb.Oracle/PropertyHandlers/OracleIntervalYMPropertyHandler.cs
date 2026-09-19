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
    /// A property handler that maps the Oracle <c>INTERVAL YEAR TO MONTH</c> type into an <see cref="OracleIntervalYM"/> property. As the driver returns the column as a total number of months, this handler exposes the years and months structure.
    /// </summary>
    public class OracleIntervalYMPropertyHandler : IPropertyHandler<object, OracleIntervalYM>
    {
        /// <summary>
        /// Converts the <c>INTERVAL YEAR TO MONTH</c> value, as returned by the driver, into an <see cref="OracleIntervalYM"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (an <see cref="OracleIntervalYM"/>, or the total number of months as a <see cref="long"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="OracleIntervalYM"/>, or <see cref="OracleIntervalYM.Null"/> when the value is <c>null</c>.</returns>
        public OracleIntervalYM Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return OracleIntervalYM.Null;
                case OracleIntervalYM interval:
                    return interval;
                default:
                    return new OracleIntervalYM(Convert.ToInt64(input));
            }
        }

        /// <summary>
        /// Converts the <see cref="OracleIntervalYM"/> into the value written into the <c>INTERVAL YEAR TO MONTH</c> column.
        /// </summary>
        /// <param name="input">The <see cref="OracleIntervalYM"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="OracleIntervalYM"/> (boxed), or <c>null</c> when its <see cref="OracleIntervalYM.IsNull"/> is <c>true</c>.</returns>
        public object Set(OracleIntervalYM input,
            PropertyHandlerSetOptions options) =>
            input.IsNull ? null : (object)input;
    }
}
