using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies the <i>OR</i> grouping for the query.
    /// </summary>
    public class OrQueryGroup : QueryGroup
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.OrQueryGroup</i> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be included on the <i>OR</i> grouping for the query.</param>
        /// <param name="queryGroups">The list of the child groupings.</param>
        public OrQueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null)
            : base(queryFields, queryGroups, Conjunction.Or)
        {
        }
    }
}