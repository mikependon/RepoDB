#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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
        /// Gets or sets the number of rows that the execution reports as affected.
        /// </summary>
        public int RowsAffected { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the command: it takes the delay, then it throws the failure or reports the rows that were affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
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
            return RowsAffected;
        }

        #endregion
    }
}
