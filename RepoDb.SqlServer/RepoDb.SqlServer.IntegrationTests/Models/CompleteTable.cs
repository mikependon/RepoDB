namespace RepoDb.SqlServer.IntegrationTests.Models
{
    public class CompleteTable
    {
        public System.Int32 Id { get; set; }
        public System.Guid SessionId { get; set; }
        public System.Int64 ColumnBigInt { get; set; }
        public System.Byte[] ColumnBinary { get; set; }
        public System.Boolean ColumnBit { get; set; }
        public System.String ColumnChar { get; set; }
        public System.DateTime ColumnDate { get; set; }
        public System.DateTime ColumnDateTime { get; set; }
        public System.DateTime ColumnDateTime2 { get; set; }
        public System.DateTimeOffset ColumnDateTimeOffset { get; set; }
        public System.Decimal ColumnDecimal { get; set; }
        public System.Double ColumnFloat { get; set; }
        //public System.Object ColumnGeography { get; set; }
        //public System.Object ColumnGeometry { get; set; }
        //public System.Object ColumnHierarchyId { get; set; }
        public System.Byte[] ColumnImage { get; set; }
        public System.Int32 ColumnInt { get; set; }
        public System.Decimal ColumnMoney { get; set; }
        public System.String ColumnNChar { get; set; }
        public System.String ColumnNText { get; set; }
        public System.Decimal ColumnNumeric { get; set; }
        public System.String ColumnNVarChar { get; set; }
        public System.Single ColumnReal { get; set; }
        public System.DateTime ColumnSmallDateTime { get; set; }
        public System.Int16 ColumnSmallInt { get; set; }
        public System.Decimal ColumnSmallMoney { get; set; }
        public System.Object ColumnSqlVariant { get; set; }
        public System.String ColumnText { get; set; }
        public System.TimeSpan ColumnTime { get; set; }
        //public System.Byte[] ColumnTimestamp { get; set; }
        public System.Byte ColumnTinyInt { get; set; }
        public System.Guid ColumnUniqueIdentifier { get; set; }
        public System.Byte[] ColumnVarBinary { get; set; }
        public System.String ColumnVarChar { get; set; }
        public System.String ColumnXml { get; set; }
    }
}
