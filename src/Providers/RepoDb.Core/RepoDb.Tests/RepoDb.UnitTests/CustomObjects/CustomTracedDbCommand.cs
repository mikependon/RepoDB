#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// A command that can be told to take some time and to fail when it is executed, so the tracing of the executions can be tested.
    /// </summary>
    public class CustomTracedDbCommand : CustomDbCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the time that the execution takes.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the exception that the execution throws, or <c>null</c> if the execution succeeds.
        /// </summary>
        public Exception Failure { get; set; }

        /// <summary>
        /// Gets or sets the action that is called each time that the command is executed.
        /// </summary>
        public Action OnExecute { get; set; }

        /// <summary>
        /// Gets or sets the factory used to create a reader for result-consumption tests.
        /// </summary>
        public Func<DbDataReader> ReaderFactory { get; set; }

        /// <summary>
        /// Gets or sets the number of rows that the execution reports as affected.
        /// </summary>
        public int RowsAffected { get; set; }

        /// <summary>
        /// Gets or sets the value that the execution of a scalar statement returns.
        /// </summary>
        public object ScalarValue { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the command: it takes the delay, then it throws the failure or reports the rows that were affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            Execute();
            return RowsAffected;
        }

        /// <summary>
        /// Executes the command as a scalar statement: it takes the delay, then it throws the failure or returns the scalar value.
        /// </summary>
        /// <returns>The scalar value.</returns>
        public override object ExecuteScalar()
        {
            Execute();
            return ScalarValue;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes the command as a query: it takes the delay, then it throws the failure or returns a reader that has no rows.
        /// </summary>
        /// <param name="behavior">The behavior of the execution.</param>
        /// <returns>The reader.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            Execute();
            return ReaderFactory?.Invoke() ?? base.ExecuteDbDataReader(behavior);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Counts the execution, then takes the delay and throws the failure.
        /// </summary>
        private void Execute()
        {
            OnExecute?.Invoke();
            if (Delay > TimeSpan.Zero)
            {
                Thread.Sleep(Delay);
            }
            if (Failure != null)
            {
                throw Failure;
            }
        }

        #endregion
    }
}
