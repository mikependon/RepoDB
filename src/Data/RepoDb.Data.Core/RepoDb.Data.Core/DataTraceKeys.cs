#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data
{
    /// <summary>
    /// A class that holds the constant values of the data operation tracking keys.
    /// </summary>
    public static class DataTraceKeys
    {
        #region Constants

        /// <summary>
        /// The trace key for the 'CopyDataTo' operation.
        /// </summary>
        public const string CopyDataTo = "CopyDataTo";

        /// <summary>
        /// The trace key for the 'ImportDataFromCsv' operation.
        /// </summary>
        public const string ImportDataFromCsv = "ImportDataFromCsv";

        /// <summary>
        /// The trace key for the 'ImportDataFromJson' operation.
        /// </summary>
        public const string ImportDataFromJson = "ImportDataFromJson";

        /// <summary>
        /// The trace key for the 'ImportDataFromXml' operation.
        /// </summary>
        public const string ImportDataFromXml = "ImportDataFromXml";

        /// <summary>
        /// The trace key for the 'MoveDataTo' operation.
        /// </summary>
        public const string MoveDataTo = "MoveDataTo";

        #endregion
    }
}
