#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;
        public bool IsUseUpsertForMergeOperation => false;
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool AreTableHintsSupported => true;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }

    public class CustomNonHintsSupportingDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;
        public bool IsUseUpsertForMergeOperation => false;
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool AreTableHintsSupported => false;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }

    public class CustomSingleStatementSupportDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => false;
        public bool IsUseUpsertForMergeOperation => false;
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool AreTableHintsSupported => true;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }
}
