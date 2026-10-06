#region Copyright Attributions

// Copyright (c) 2024 Bert Huijben and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace System.Runtime.CompilerServices
{
#if !NET
    // Required to allow init properties in netstandard
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class IsExternalInit : Attribute
    {
    }
#endif
}

namespace System.Diagnostics.CodeAnalysis
{
#if !NET
    // Trimming and AOT annotations, only recognized by the trimmer/ILC on .NET, but declared here so that
    // the netstandard2.0 build shares the same annotated source.
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method |
        AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
        Inherited = false)]
    internal sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes) => MemberTypes = memberTypes;

        public DynamicallyAccessedMemberTypes MemberTypes { get; }
    }

    [Flags]
    internal enum DynamicallyAccessedMemberTypes
    {
        None = 0,
        PublicParameterlessConstructor = 0x0001,
        PublicConstructors = 0x0002 | PublicParameterlessConstructor,
        NonPublicConstructors = 0x0004,
        PublicMethods = 0x0008,
        NonPublicMethods = 0x0010,
        PublicFields = 0x0020,
        NonPublicFields = 0x0040,
        PublicNestedTypes = 0x0080,
        NonPublicNestedTypes = 0x0100,
        PublicProperties = 0x0200,
        NonPublicProperties = 0x0400,
        PublicEvents = 0x0800,
        NonPublicEvents = 0x1000,
        Interfaces = 0x2000,
        All = ~None
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    internal sealed class RequiresUnreferencedCodeAttribute : Attribute
    {
        public RequiresUnreferencedCodeAttribute(string message) => Message = message;

        public string Message { get; }

        public string Url { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    internal sealed class RequiresDynamicCodeAttribute : Attribute
    {
        public RequiresDynamicCodeAttribute(string message) => Message = message;

        public string Message { get; }

        public string Url { get; set; }
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class DynamicDependencyAttribute : Attribute
    {
        public DynamicDependencyAttribute(string memberSignature, string typeName, string assemblyName)
        {
            MemberSignature = memberSignature;
            TypeName = typeName;
            AssemblyName = assemblyName;
        }

        public string MemberSignature { get; }

        public string TypeName { get; }

        public string AssemblyName { get; }

        public string Condition { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    internal sealed class UnconditionalSuppressMessageAttribute : Attribute
    {
        public UnconditionalSuppressMessageAttribute(string category, string checkId)
        {
            Category = category;
            CheckId = checkId;
        }

        public string Category { get; }

        public string CheckId { get; }

        public string Scope { get; set; }

        public string Target { get; set; }

        public string MessageId { get; set; }

        public string Justification { get; set; }
    }
#endif
}
