#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// 
    /// </summary>
    internal class InsertExecutionContext
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<DbField> InputFields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<DbCommand, object> ParametersSetterFunc { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Action<object, object> KeyPropertySetterFunc { get; set; }

        /// <summary>
        /// The name of the key field targeted by <see cref="KeyPropertySetterFunc"/>, if any.
        /// </summary>
        public string KeyFieldName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HasIdentityKey { get; set; }
    }
}
