using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.IntegrationTests.Models
{
    //https://stackoverflow.com/questions/5873170/generate-class-from-database-table
    [Map("[dbo].[TypeMap]")]
    public class TypeMap: DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        public Guid SessionId { get; set; }

        public long? bigint_column { get; set; }

        public bool? bit_column { get; set; }

        public string char_column { get; set; }

        public DateTime? date_column { get; set; }

        public DateTime? datetime_column { get; set; }

        public DateTime? datetime2_column { get; set; }

        public DateTimeOffset? datetimeoffset_column { get; set; }

        public decimal? decimal_column { get; set; }

        public double? float_column { get; set; }

        public int? int_column { get; set; }

        public decimal? money_column { get; set; }

        public string nchar_column { get; set; }

        public string ntext_column { get; set; }

        public decimal? numeric_column { get; set; }

        public string nvarchar_column { get; set; }

        public string nvarcharmax_column { get; set; }

        public double? real_column { get; set; }

        public DateTime? smalldatetime_column { get; set; }

        public short? smallint_column { get; set; }

        public decimal? smallmoney_column { get; set; }

        public string text_column { get; set; }

        public TimeSpan? time_column { get; set; }

        public byte? tinyint_column { get; set; }

        public Guid? uniqueidentifier { get; set; }

        public string varchar_column { get; set; }

        public string varcharmax_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapBlob : DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        public Guid SessionId { get; set; }

        public byte[] binary_column { get; set; }

        public byte[] image_column { get; set; }

        public byte[] varbinary_column { get; set; }

        public byte[] varbinarymax_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapXml : DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        public Guid SessionId { get; set; }

        public string xml_column { get; set; }
    }

    [Map("[dbo].[TypeMap]")]
    public class TypeMapSpatial : DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        public Guid SessionId { get; set; }

        public object geography_column { get; set; }

        public object geometry_column { get; set; }
    }
    [Map("[dbo].[TypeMap]")]
    public class TypeMapUnsupported : DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        public Guid SessionId { get; set; }

        public object hierarchyid_column { get; set; }

        public object sql_variant_column { get; set; }
    }
}
