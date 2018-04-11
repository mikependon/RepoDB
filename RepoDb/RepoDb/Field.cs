using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using System;

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

        public static IEnumerable<IField> Parse(object obj)
        {
            if (obj == null)
            {
                throw new InvalidOperationException("Parameter 'obj' cannot be null.");
            }
            var list = new List<IField>();
            obj.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    list.Add(new Field(property.Name));
                });
            return list;
        }
    }
}