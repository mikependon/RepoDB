using RepoDb.Interfaces;

namespace RepoDb
{
    public class Parameter : IParameter
    {
        public Parameter(string name, object value)
        {
            Name = $"_{name}";
            Value = value;
        }

        public string Name { get; internal set; }

        public object Value { get; }

        public override string ToString()
        {
            return $"@{Name} ({Value})";
        }
    }
}