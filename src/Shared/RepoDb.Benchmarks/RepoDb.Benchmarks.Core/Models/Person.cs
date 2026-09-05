#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.Benchmarks.Core.Models
{
    [Table("Person")]
    public class Person
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime CreatedDateUtc { get; set; }
    }
}
