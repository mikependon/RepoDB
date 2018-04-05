using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class Field : IField
    {
        public Field(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static IEnumerable<IField> From(params string[] fields)
        {
            return fields.ToList().Select(field => new Field(field));
        }

        public override string ToString()
        {
            return $"[{Name}]";
        }
    }
}