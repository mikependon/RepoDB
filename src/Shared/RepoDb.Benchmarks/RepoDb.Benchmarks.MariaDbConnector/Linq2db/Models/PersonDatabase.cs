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

	[Table(Name="Person")]
	public partial class Person
	{
		[PrimaryKey, Identity] public long     Id             { get; set; } // bigint
		[Column,     NotNull ] public string   Name           { get; set; } // varchar(128)
		[Column,     NotNull ] public int      Age            { get; set; } // int
		[Column,     NotNull ] public DateTime CreatedDateUtc { get; set; } // datetime(5)
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
