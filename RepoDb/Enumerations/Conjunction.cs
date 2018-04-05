using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    public enum Conjunction : short
    {
        [Text("AND")] And = 1,
        [Text("OR")] Or = 2
    }
}
