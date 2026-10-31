#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// A connection that has a database, and that creates commands that can be told to take some time and to fail, so the tracing of the executions can be tested.
    /// </summary>
    public class CustomTracedDbConnection : CustomDbConnection
    {
        #region Properties

        /// <summary>
        /// Gets the name of the database of the connection.
        /// </summary>
        public override string Database => "TestDatabase";

        /// <summary>
        /// Gets or sets the time that the execution of a command takes.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets the number of times that a command of the connection was executed.
        /// </summary>
        public int ExecutionCount { get; private set; }

        /// <summary>
        /// Gets or sets the exception that the execution of a command throws, or <c>null</c> if the execution succeeds.
        /// </summary>
        public Exception Failure { get; set; }

        /// <summary>
        /// Gets or sets the factory used to create command readers.
        /// </summary>
        public Func<DbDataReader> ReaderFactory { get; set; }

        /// <summary>
        /// Gets or sets the number of rows that the execution of a command reports as affected.
        /// </summary>
        public int RowsAffected { get; set; }

        /// <summary>
        /// Gets or sets the value that the execution of a scalar statement returns.
        /// </summary>
        public object ScalarValue { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a command that is configured as the connection is.
        /// </summary>
        /// <returns>The command.</returns>
        protected override DbCommand CreateDbCommand() =>
            new CustomTracedDbCommand
            {
                Connection = this,
                Delay = Delay,
                Failure = Failure,
                OnExecute = () => ExecutionCount++,
                ReaderFactory = ReaderFactory,
                RowsAffected = RowsAffected,
                ScalarValue = ScalarValue
            };

        #endregion
    }
}
