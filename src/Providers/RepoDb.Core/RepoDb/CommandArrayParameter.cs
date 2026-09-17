#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the array value of the parameter.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="CommandArrayParameter"/> class.
    /// </remarks>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="values">The values of the parameter.</param>
    internal class CommandArrayParameter(string parameterName,
        IEnumerable<object> values)
    {

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string ParameterName { get; } = parameterName;

        /// <summary>
        /// Gets the values of the parameter.
        /// </summary>
        public IEnumerable<object> Values { get; } = values;
    }
}
