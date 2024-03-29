﻿using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Utilities;
using CodeStandart.Analyzers;

namespace CodeStandart.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndividedLocalDeclarationCodeFixProvider)), Shared]
    public class EachEnumMemberFromNewStringCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Переместить все элементы перечисления на свою строку";

        public sealed override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create(CREO016_EachEnumFromNewStringAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var _enum = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<EnumDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => MakeLinesForEnumMembers(context.Document, _enum, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        

        private async Task<Document> MakeLinesForEnumMembers(Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            var members = enumDeclaration.Members;

            var newEnum = SyntaxFactory.EnumDeclaration(
                enumDeclaration.AttributeLists,
                enumDeclaration.Modifiers,
                enumDeclaration.Identifier,
                enumDeclaration.BaseList,
                new SeparatedSyntaxList<EnumMemberDeclarationSyntax>());

            foreach (var member in members)
            {
                newEnum = newEnum.AddMembers(member.WithoutTrivia());
            }
            
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root.ReplaceNode(enumDeclaration, newEnum);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
