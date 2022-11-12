using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Mapping;

namespace DataModels
{
	public partial class RepoDbDB : LinqToDB.Data.DataConnection
	{
		public ITable<Person> People => this.GetTable<Person>();

		partial void InitMappingSchema()
		{
		}

		public RepoDbDB()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public RepoDbDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public RepoDbDB(LinqToDBConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	[Table(Schema="public", Name="Person")]
	public partial class Person
	{
		[PrimaryKey, NotNull    ] public long      Id             { get; set; } // bigint
		[Column,        Nullable] public string    Name           { get; set; } // character varying(128)
		[Column,        Nullable] public int?      Age            { get; set; } // integer
		[Column,        Nullable] public DateTime? CreatedDateUtc { get; set; } // timestamp (5) without time zone
	}

	public static partial class TableExtensions
	{
		public static Person Find(this ITable<Person> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
