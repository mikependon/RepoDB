using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface IQueryGroup
    {
        // Properties
        Conjunction Conjunction { get; }
        IEnumerable<IQueryField> QueryFields { get; }
        IEnumerable<IQueryGroup> QueryGroups { get; }
        // Methods
        IEnumerable<IQueryField> GetAllQueryFields();
        IQueryGroup Fix();
        string GetConjunctionText();
        string GetString();
    }
}
