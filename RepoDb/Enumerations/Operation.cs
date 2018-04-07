using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    public enum Operation : short
    {
        [Text("=")] Equal,
        [Text("<>")] NotEqual,
        [Text("<")] LessThan,
        [Text(">")] GreaterThan,
        [Text("<=")] LessThanOrEqual,
        [Text(">=")] GreaterThanOrEqual,
        [Text("LIKE")] Like,
        [Text("NOT LIKE")] NotLike,
        [Text("BETWEEN")] Between,
        [Text("NOT BETWEEN")] NotBetween,
        [Text("IN")] In,
        [Text("NOT IN")] NotIn
    }
}