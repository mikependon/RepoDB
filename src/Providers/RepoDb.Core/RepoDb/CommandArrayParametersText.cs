#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the command text of the array value of the parameter.
    /// </summary>
    internal class CommandArrayParametersText
    {
        /// <summary>
        /// Gets the actual command string to be executed (derived from array parameters).
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets the database type of the parameter.
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets the list of the command array parameters.
        /// </summary>
        public IList<CommandArrayParameter> CommandArrayParameters { get; } = new List<CommandArrayParameter>();
    }
}
