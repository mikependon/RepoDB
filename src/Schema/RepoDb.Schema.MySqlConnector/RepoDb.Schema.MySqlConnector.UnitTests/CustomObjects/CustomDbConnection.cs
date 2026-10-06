#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.DbSettings;

namespace RepoDb.Schema.MySqlConnector.UnitTests.CustomObjects
{
    public class CustomDbConnection : System.Data.Common.DbConnection, IDbConnection
    {
        static CustomDbConnection()
        {
            DbSettingMapper.Add<CustomDbConnection>(new MySqlConnectorDbSetting(), true);
        }

        public override string ConnectionString { get; set; }

        public List<string> ExecutedCommands { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the condition that makes an executed command fail (with an <see cref="System.InvalidOperationException"/>).
        /// </summary>
        public System.Func<string, bool> FailWhen { get; set; }

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

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new CustomDbTransaction();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new CustomDbCommand() { Connection = this };
        }
    }
}
