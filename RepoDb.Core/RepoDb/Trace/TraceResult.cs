#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal class TraceResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public TraceResult(CancellableTraceLog log)
        {
            SessionId = log.SessionId;
            StartTime = log.StartTime;
            CancellableTraceLog = log;
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 
        /// </summary>
        public CancellableTraceLog CancellableTraceLog { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static TraceResult Create(string? key,
            DbCommand command) =>
            new TraceResult(
                new CancellableTraceLog(Guid.NewGuid(),
                    key, command.CommandText, GetParameters(command)));

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
