#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Data.Common;
using Moq;
using RepoDb.Interfaces;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class CustomDbConnection : DbConnection
    {
        static CustomDbConnection()
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(s => s.OpeningQuote).Returns("[");
            setting.SetupGet(s => s.ClosingQuote).Returns("]");
            setting.SetupGet(s => s.DefaultSchema).Returns("dbo");
            DbSettingMapper.Add<CustomDbConnection>(setting.Object, true);
        }

        public override string ConnectionString { get; set; }

        public override string Database { get; }

        public override string DataSource { get; }

        public override string ServerVersion { get; }

        public override ConnectionState State { get; }

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            null;

        protected override DbCommand CreateDbCommand() =>
            null;
    }
}
