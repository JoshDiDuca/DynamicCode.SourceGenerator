﻿using System.Collections.Generic;

namespace DynamicCode.SourceGenerator.Metadata.Interfaces
{
    public interface IEnumMetadata : INamedItem
    {
        string DocComment { get; }
        string Namespace { get; }
        ITypeMetadata Type { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        IClassMetadata ContainingClass { get; }
        IEnumerable<IEnumValueMetadata> Values { get; }
    }
}