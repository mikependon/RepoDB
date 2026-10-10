#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.StatementBuilders;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeStatementBuilder : BaseStatementBuilder
    {
        public FakeStatementBuilder()
            : base(new FakeDbSetting(), null, null)
        {
        }

        public override string CreateBatchQuery(string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup where = null, string hints = null) =>
            string.Empty;

        public override string CreateMerge(string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, DbField primaryField = null, DbField identityField = null, string hints = null) =>
            string.Empty;

        public override string CreateMergeAll(string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, int batchSize = 10, DbField primaryField = null, DbField identityField = null, string hints = null) =>
            string.Empty;

        public override string CreateSkipQuery(string tableName, IEnumerable<Field> fields, int skip, int take, IEnumerable<OrderField> orderBy = null, QueryGroup where = null, string hints = null) =>
            string.Empty;
    }
}
