using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Exceptions
{
    public class DataEntityMapException : Exception
    {
        public DataEntityMapException(Command command)
            : base(command.ToString().ToUpper()) { }
    }
}
