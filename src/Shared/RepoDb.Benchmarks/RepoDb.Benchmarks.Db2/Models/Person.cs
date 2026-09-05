#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Dapper.Contrib.Extensions;
using RepoDb.Attributes;

namespace RepoDb.Benchmarks.Db2.Models
{
    // DB2 folds unquoted identifiers to uppercase, but RepoDb and Linq2Db quote identifiers
    // (case-sensitively) by default, so every mapped name below is spelled out in the exact
    // uppercase form the table/columns are physically stored as. [Table] drives Dapper.Contrib's
    // GetAll<T>() (it does not honor RepoDb's [Map] and pluralizes the class name otherwise);
    // [Map] drives RepoDb (which does not honor Dapper.Contrib's [Table]).
    [Table("PERSON")]
    [Map("PERSON")]
    public class Person
    {
        [Key]
        [Map("ID")]
        public virtual long Id { get; set; }

        [Map("NAME")]
        public virtual string Name { get; set; }

        [Map("AGE")]
        public virtual int Age { get; set; }

        [Map("CREATEDDATEUTC")]
        public virtual DateTime CreatedDateUtc { get; set; }
    }
}
