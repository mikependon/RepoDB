using System;
using System.Data;
using RepoDb.Attributes;

namespace RepoDb.IntegrationTests.Models
{
    //https://stackoverflow.com/questions/5873170/generate-class-from-database-table
    [Map("[dbo].[TypeMap]")]
    public class TypeMap
    {
        public Guid SessionId { get; set; }

        public string char_column { get; set; }
        
        public string nchar_column { get; set; }

        public string ntext_column { get; set; }

        public string nvarchar_column { get; set; }

        public string nvarcharmax_column { get; set; }
        
        public string text_column { get; set; }

        public Guid? uniqueidentifier { get; set; }

        public string varchar_column { get; set; }

        public string varcharmax_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapNumeric
    {
        public Guid SessionId { get; set; }

        public long? bigint_column { get; set; }

        public bool? bit_column { get; set; }

        public decimal? decimal_column { get; set; }

        public double? float_column { get; set; }

        public int? int_column { get; set; }

        public decimal? money_column { get; set; }

        public decimal? numeric_column { get; set; }

        public Single? real_column { get; set; }

        public byte? tinyint_column { get; set; }

        public short? smallint_column { get; set; }

        public decimal? smallmoney_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapDate
    {
        public Guid SessionId { get; set; }

        public DateTime? date_column { get; set; }

        public DateTime? datetime_column { get; set; }

        public DateTime? datetime2_column { get; set; }

        public DateTimeOffset? datetimeoffset_column { get; set; }

        public DateTime? smalldatetime_column { get; set; }

        public TimeSpan? time_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapBlob
    {
        public Guid SessionId { get; set; }

        [TypeMap(DbType.Binary)]
        public byte[] binary_column { get; set; }

        public byte[] image_column { get; set; }

        public byte[] varbinary_column { get; set; }

        public byte[] varbinarymax_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapXml
    {
        public Guid SessionId { get; set; }

        public string xml_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapSpatial
    {
        public Guid SessionId { get; set; }

        public object geography_column { get; set; }

        public object geometry_column { get; set; }
    }
    [Map("[dbo].[TypeMap]")]
    public class TypeMapUnsupported
    {
        public Guid SessionId { get; set; }

        public object hierarchyid_column { get; set; }

        public object sql_variant_column { get; set; }
    }
}
