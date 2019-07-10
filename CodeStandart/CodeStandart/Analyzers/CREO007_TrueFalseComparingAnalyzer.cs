using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Creo007_TrueFalseComparingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Creo007";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.CREO007_AnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO007_AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO007_AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literal = context.Node;

            var parent = literal.Parent;

            if (parent.Kind() != SyntaxKind.EqualsExpression && parent.Kind() != SyntaxKind.NotEqualsExpression)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                _rule,
                literal.GetLocation(),
                literal.Parent,
                literal));
        }
    }
}