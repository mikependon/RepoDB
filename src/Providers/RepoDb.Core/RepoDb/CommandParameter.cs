#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to hold the definition of the <see cref="DbCommand"/> parameters.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="CommandParameter"/> class.
    /// </remarks>
    /// <param name="field">The <see cref="Field"/> object that is connected.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="mappedToType">The parent type where this parameter is mapped.</param>
    internal class CommandParameter(Field field,
        object value,
        Type mappedToType)
    {

        /// <summary>
        /// The field that is connected.
        /// </summary>
        public Field Field { get; set; } = field;

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public object Value { get; set; } = value;

        /// <summary>
        /// The parent type where this parameter is mapped.
        /// </summary>
        public Type MappedToType { get; set; } = mappedToType;
    }
}
