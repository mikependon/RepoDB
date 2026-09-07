#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Globalization;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.Benchmarks.Sqlite.Microsoft.PropertyHandlers
{
    // SQLite has no native date/time storage class - Microsoft.Data.Sqlite stores DATETIME columns as
    // plain TEXT and, unlike EF Core's or linq2db's own SQLite providers, does not convert them back to
    // System.DateTime on read (RepoDb.Sqlite.Microsoft's own MdsSqLiteDbTypeNameToClientTypeResolver
    // documents this - "datetime" resolves to typeof(string)). Without this handler, RepoDb's compiled
    // reader throws trying to coerce the raw string straight into the DateTime-typed Person.CreatedDateUtc
    // property.
    public class SqliteDateTimePropertyHandler : IPropertyHandler<string, DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss.ffffff";

        public DateTime Get(string input,
            PropertyHandlerGetOptions options) =>
            DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.None);

        public string Set(DateTime input,
            PropertyHandlerSetOptions options) =>
            input.ToString(Format, CultureInfo.InvariantCulture);
    }
}
