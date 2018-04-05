using System;
using RepoDb.Enumerations;

namespace RepoDb.Attributes
{
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute(Command command)
        {
            Command = command;
        }

        public Command Command { get; }
    }
}