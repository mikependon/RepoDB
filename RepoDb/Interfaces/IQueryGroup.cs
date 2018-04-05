using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface IQueryGroup
    {
        Conjunction Conjunction { get; }
        IEnumerable<IQueryField> QueryFields { get; }
        IEnumerable<IQueryGroup> QueryGroups { get; }
        IEnumerable<IQueryField> GetAllQueryFields();
        IQueryGroup FixParameters();
        string GetString();
    }
}
