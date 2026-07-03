using System;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// A class that is used to define the telemetry data to be published to the insights solution.
    /// </summary>
    public class TelemetryItem
    {

        /// <summary>
        /// The group name of the telemetry item. This is used to group the telemetry items.
        /// </summary>
        public string Group { get; set; } = "Default";

        /// <summary>
        /// The name of the application that produces the telemetry.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// The session identifier of the operation (i.e. Insert, Delete, Update, Query) that has been executed.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// The name of the operation that has been executed. This can be Insert, Delete, Update, Query, etc.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// The start time of when the operation has started the execution. This is used to calculate the elapsed time of the operation.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The actual statement that has been executed. This can be the SQL statement or the query expression that has been executed.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// The total elapsed time of the operation that has been executed. This is calculated by subtracting the <see cref="StartTime"/> from the current time.
        /// </summary>
        public double Elapsed { get; set; }

        /// <summary>
        /// The flag that indicates whether the operation has been cancelled or not. This is used to determine if the operation has been cancelled by the user or by the system.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// The client machine name that has executed the operation. This is used to identify the machine that has executed the operation.
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// The client machine name that has executed the operation. This is used to identify the machine that has executed the operation.
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// The client machine name that has executed the operation. This is used to identify the machine that has executed the operation.
        /// </summary>
        public string Version { get; set; }
    }
}
