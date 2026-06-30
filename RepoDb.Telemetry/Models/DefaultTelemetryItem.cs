using System;

namespace RepoDb.Telemetry.Default.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal class DefaultTelemetryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCancelled{ get; set; }

    }
}
