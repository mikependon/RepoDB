using RepoDb.Enumerations;

namespace RepoDb.Exceptions
{
    public class DuplicateDataEntityMapException : DataEntityMapException
    {
        public DuplicateDataEntityMapException(Command command)
            : base(command) { }
    }
}
