using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class AndQueryGroup : QueryGroup
    {
        public AndQueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null)
            : base(queryFields, queryGroups, Conjunction.And)
        {
        }
    }
}