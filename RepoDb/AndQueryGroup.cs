using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies the <i>AND</i> grouping for the query.
    /// </summary>
    public class AndQueryGroup : QueryGroup
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.AndQueryGroup</i> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be included on the <i>AND</i> grouping for the query.</param>
        /// <param name="queryGroups">The list of the child groupings.</param>
        public AndQueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null)
            : base(queryFields, queryGroups, Conjunction.And)
        {
        }
    }
}