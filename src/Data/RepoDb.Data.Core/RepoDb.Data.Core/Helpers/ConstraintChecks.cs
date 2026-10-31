#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Schema;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Stops the destination database from checking the foreign keys while the rows are copied, and makes it check them again afterwards,
    /// with the statements that the schema composer of the destination connection composes (see <see cref="IConstraintCheckSupport"/>).
    /// </summary>
    internal static class ConstraintChecks
    {
        #region Public Methods

        /// <summary>
        /// Validates the behavior and gets the support of the destination connection that is needed to apply it.
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="behavior">The behavior that defines whether the foreign keys are checked while the rows are copied.</param>
        /// <returns>The support of the schema composer of the destination connection, or <c>null</c> if the foreign keys are enforced.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The behavior is not defined.</exception>
        /// <exception cref="NotSupportedException">The foreign keys are to be deferred, but the destination does not support it.</exception>
        internal static IConstraintCheckSupport Resolve(IDbConnection destinationConnection,
            ConstraintCheckBehavior behavior)
        {
            if (!Enum.IsDefined(typeof(ConstraintCheckBehavior),
                behavior))
            {
                throw new ArgumentOutOfRangeException(nameof(behavior));
            }

            if (behavior == ConstraintCheckBehavior.Enforce)
            {
                return null;
            }

            if (SchemaComposerMapper.Get(destinationConnection) is IConstraintCheckSupport support)
            {
                return support;
            }

            throw new NotSupportedException($"The destination connection ({destinationConnection.GetType().Name}) does not support {nameof(ConstraintCheckBehavior)}.{nameof(ConstraintCheckBehavior.Defer)}: its database can not stop checking the foreign keys for a session.");
        }

        /// <summary>
        /// Stops the destination database from checking the foreign keys, if the support is given.
        /// </summary>
        /// <param name="support">The support of the destination connection, or <c>null</c> to do nothing.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static void Disable(IConstraintCheckSupport support,
            IDbConnection destinationConnection,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            if (support == null)
            {
                return;
            }

            destinationConnection.ExecuteNonQuery(support.ComposeDisableConstraintChecks(transaction != null),
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Asynchronously stops the destination database from checking the foreign keys, if the support is given.
        /// </summary>
        /// <param name="support">The support of the destination connection, or <c>null</c> to do nothing.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static async Task DisableAsync(IConstraintCheckSupport support,
            IDbConnection destinationConnection,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            if (support == null)
            {
                return;
            }

            await destinationConnection.ExecuteNonQueryAsync(support.ComposeDisableConstraintChecks(transaction != null),
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes the destination database check the foreign keys again, if the support is given. It always runs once the rows are copied, whether the copy succeeded or not,
        /// so a connection is never given back (i.e.: to a pool) with its foreign keys not checked.
        /// </summary>
        /// <param name="support">The support of the destination connection, or <c>null</c> to do nothing.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="copyFailed">Whether the copy has failed: if it has, a failure to check the foreign keys again is not thrown, so it does not hide the failure of the copy.</param>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static void Enable(IConstraintCheckSupport support,
            IDbConnection destinationConnection,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            bool copyFailed)
        {
            if (support == null)
            {
                return;
            }

            try
            {
                destinationConnection.ExecuteNonQuery(support.ComposeEnableConstraintChecks(transaction != null),
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace);
            }
            catch when (copyFailed)
            {
            }
        }

        /// <summary>
        /// Asynchronously makes the destination database check the foreign keys again, if the support is given. It always runs once the rows are copied (it is not cancelled),
        /// whether the copy succeeded or not.
        /// </summary>
        /// <param name="support">The support of the destination connection, or <c>null</c> to do nothing.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="copyFailed">Whether the copy has failed: if it has, a failure to check the foreign keys again is not thrown, so it does not hide the failure of the copy.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static async Task EnableAsync(IConstraintCheckSupport support,
            IDbConnection destinationConnection,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            bool copyFailed)
        {
            if (support == null)
            {
                return;
            }

            try
            {
                await destinationConnection.ExecuteNonQueryAsync(support.ComposeEnableConstraintChecks(transaction != null),
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
            catch when (copyFailed)
            {
            }
        }

        #endregion
    }
}
