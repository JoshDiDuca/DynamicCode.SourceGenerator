﻿using System;
using System.Collections.Generic;
using System.Linq;
using DynamicCode.SourceGenerator.Models.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DynamicCode.SourceGenerator.Metadata.Interfaces;

namespace DynamicCode.SourceGenerator.Metadata.Roslyn
{
    public class RoslynFileMetadata : IFileMetadata
    {
        private readonly Action<string[]> _requestRender;
        private Document _document;
        private SyntaxNode _root;
        private SemanticModel _semanticModel;

        public RoslynFileMetadata(Document document, CodeGenerationConfig settings, Action<string[]> requestRender)
        {
            _requestRender = requestRender;

            LoadDocument(document);
            Settings = settings;
        }

        public CodeGenerationConfig Settings { get; }
        public string Name => _document.Name;
        public string FullName => _document.FilePath;

        public IEnumerable<IClassMetadata> Classes => RoslynClassMetadata.FromNamedTypeSymbols(GetNamespaceChildNodes<ClassDeclarationSyntax>(), this);
        public IEnumerable<IDelegateMetadata> Delegates => RoslynDelegateMetadata.FromNamedTypeSymbols(GetNamespaceChildNodes<DelegateDeclarationSyntax>());
        public IEnumerable<IEnumMetadata> Enums => RoslynEnumMetadata.FromNamedTypeSymbols(GetNamespaceChildNodes<EnumDeclarationSyntax>());
        public IEnumerable<IInterfaceMetadata> Interfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(GetNamespaceChildNodes<InterfaceDeclarationSyntax>(), this);

        private void LoadDocument(Document document)
        {
            _document = document;
            _semanticModel = document.GetSemanticModelAsync().Result;
            _root = _semanticModel.SyntaxTree.GetRoot();
        }

        private IEnumerable<INamedTypeSymbol> GetNamespaceChildNodes<T>() where T : SyntaxNode
        {
            var symbols = _root.ChildNodes().OfType<T>().Concat(
                _root.ChildNodes().OfType<NamespaceDeclarationSyntax>().SelectMany(n => n.ChildNodes().OfType<T>()))
                .Select(c => _semanticModel.GetDeclaredSymbol(c) as INamedTypeSymbol);
            return symbols;
        }
    }
}
