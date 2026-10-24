#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// An interceptor of the copy whose transformation is given as a function.
    /// </summary>
    public class FakeDataInterceptor : IDataInterceptor<Row>
    {
        private readonly Func<Row, Row> _transform;

        public FakeDataInterceptor(Func<Row, Row> transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// Gets the number of rows that the interceptor has received.
        /// </summary>
        public int Calls { get; private set; }

        public Row Transform(Row data)
        {
            Calls++;
            return _transform(data);
        }
    }
}
