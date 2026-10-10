#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A command that was executed on a <see cref="FakeDbConnection"/>.
    /// </summary>
    public class FakeExecution
    {
        public string CommandText { get; set; }

        public string Kind { get; set; }

        public int? CommandTimeout { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }
}
