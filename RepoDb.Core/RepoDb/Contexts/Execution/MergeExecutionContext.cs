using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// 
    /// </summary>
    internal class MergeExecutionContext
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<DbField> InputFields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<DbCommand, object> ParametersSetterFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<object, object> KeyPropertySetterFunc { get; set; }
    }
}
