using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// 
    /// </summary>
    internal class MergeAllExecutionContext
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
        public int BatchSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<DbCommand, object> SingleDataEntityParametersSetterFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<DbCommand, IList<object>> MultipleDataEntitiesParametersSetterFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<object, object> PrimaryPropertySetterFunc { get; set; }
    }
}
