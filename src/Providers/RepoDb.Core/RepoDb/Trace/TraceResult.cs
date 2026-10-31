#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="log"></param>
    internal class TraceResult(CancellableTraceLog log)
    {
        #region Private Variables

        /// <summary>
        /// Measures the length of the execution.
        /// </summary>
        private readonly Stopwatch _watch = Stopwatch.StartNew();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the length of the execution so far. It is measured with a stopwatch, so it does not depend on the clock.
        /// </summary>
        public TimeSpan Elapsed => _watch.Elapsed;

        /// <summary>
        /// 
        /// </summary>
        public Guid SessionId { get; } = log.SessionId;

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; } = log.StartTime;

        /// <summary>
        /// 
        /// </summary>
        public CancellableTraceLog CancellableTraceLog { get; } = log;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static TraceResult Create(string key,
            DbCommand command)
        {
            return new TraceResult(
                new CancellableTraceLog(Guid.NewGuid(),
                    key,
                    command.CommandText,
                    GetParameters(command),
                    TraceContext.Capture(command)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static IEnumerable<IDbDataParameter> GetParameters(DbCommand command)
        {
            var list = new List<IDbDataParameter>();

            foreach (IDbDataParameter parameter in command.Parameters)
            {
                list.Add(parameter);
            }

            return list;
        }

        #endregion
    }
}
