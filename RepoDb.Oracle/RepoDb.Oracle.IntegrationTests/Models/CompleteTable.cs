namespace RepoDb.Oracle.IntegrationTests.Models
{
    public class CompleteTable
    {
        public System.Int32 Id { get; set; }
        public System.Guid SessionId { get; set; }
        public System.String ColumnVarchar { get; set; }
        public System.Decimal ColumnNumber { get; set; }
        public System.DateTime ColumnDate { get; set; }
        public System.DateTime ColumnTimestamp { get; set; }
    }
}
