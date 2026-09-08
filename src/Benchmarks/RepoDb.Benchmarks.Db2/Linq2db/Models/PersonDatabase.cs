#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;

using LinqToDB;
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

		public RepoDbDB(DataOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	// DB2 folds unquoted identifiers to uppercase, and Linq2Db quotes identifiers (case-sensitively)
	// by default, so every mapped name below is spelled out in the exact uppercase form the
	// table/columns are physically stored as.
	[Table(Name="PERSON")]
	public partial class Person
	{
		[PrimaryKey, Identity, Column("ID")            ] public long     Id             { get; set; } // bigint
		[Column("NAME"),           NotNull             ] public string   Name           { get; set; } // varchar(128)
		[Column("AGE"),            NotNull             ] public int      Age            { get; set; } // integer
		[Column("CREATEDDATEUTC"), NotNull             ] public DateTime CreatedDateUtc { get; set; } // timestamp
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
