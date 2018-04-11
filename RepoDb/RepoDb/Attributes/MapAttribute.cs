using System;
using System.Data;

namespace RepoDb.Attributes
{
    public class MapAttribute : Attribute
    {
        public MapAttribute(string name)
        : this(name, CommandType.Text)
        {
        }

        public MapAttribute(string name, CommandType commandType)
        {
            Name = name;
            CommandType = commandType;
        }

        public string Name { get; set; }

        public CommandType CommandType { get; set; }
    }
}