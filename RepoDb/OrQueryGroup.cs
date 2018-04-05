using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class OrQueryGroup : QueryGroup
    {
        // Constructors
        public OrQueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null)
            : base(queryFields, queryGroups, Conjunction.Or)
        {
        }
    }
}