using System;

namespace RepoDb.Telemetry.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TelemetryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Group { get; set; } = "Default";

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
        public bool IsCancelled { get; set; }
    }
}
