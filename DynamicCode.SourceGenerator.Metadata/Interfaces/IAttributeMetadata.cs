﻿namespace DynamicCode.SourceGenerator.Metadata.Interfaces
{
    public interface IAttributeMetadata : INamedItem
    {
        string Value { get; }
    }
}