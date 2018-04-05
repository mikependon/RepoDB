using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    public enum Operation : short
    {
        [Text("=")] Equal,
        [Text("<>")] NotEqual, // [Text("!=")] NotEqual,
        [Text("<")] LessThan,
        [Text(">")] GreaterThan,
        [Text("<=")] LessThanOrEqual,
        [Text(">=")] GreaterThanOrEqual,
        [Text("LIKE")] Like
    }
}