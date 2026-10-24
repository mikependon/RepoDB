#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A command that a <see cref="CustomDbConnection"/> has executed.
    /// </summary>
    public class ExecutedCommand
    {
        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public System.Data.IDbTransaction Transaction { get; set; }

        public System.Collections.Generic.Dictionary<string, object> Parameters { get; set; }
    }

    public class CustomDbConnection : System.Data.Common.DbConnection, IDbConnection
    {
        static CustomDbConnection()
        {
            CustomDbSetting.Map(null);
        }

        public override string ConnectionString { get; set; }

        public List<string> ExecutedCommands { get; } = new List<string>();

        /// <summary>
        /// Gets the commands that were executed as scalars (the statements that check whether something exists).
        /// </summary>
        public List<string> ExecutedScalars { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the result of an executed scalar command.
        /// </summary>
        public System.Func<string, object> ScalarResult { get; set; }

        /// <summary>
        /// Gets or sets the condition that makes an executed command fail (with an <see cref="System.InvalidOperationException"/>).
        /// </summary>
        public System.Func<string, bool> FailWhen { get; set; }

        /// <summary>
        /// Gets the details of the commands that were executed (their text, command timeout, transaction and the values of their parameters).
        /// </summary>
        public List<ExecutedCommand> Details { get; } = new List<ExecutedCommand>();

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
            return new CustomDbTransaction(this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new CustomDbCommand() { Connection = this };
        }
    }
}
