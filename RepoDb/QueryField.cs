using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class QueryField : IQueryField
    {
        public QueryField(string fieldName, object value)
            : this(fieldName, Operation.Equal, value)
        {
        }

        public QueryField(string fieldName, Operation operation, object value)
        {
            Field = new Field(fieldName);
            Operation = operation;
            Parameter = new Parameter(fieldName, value);
        }

        public IField Field { get; }

        public Operation Operation { get; }

        public IParameter Parameter { get; }

        public override string ToString()
        {
            return $"{Field.ToString()} = {Parameter.ToString()}";
        }
    }
}