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
    public class Creo008_InterfaceNamingNoIIPrefixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO008";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.CREO008_AnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO008_AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO008_AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.TypeKind != TypeKind.Interface)
                return;

            var name = symbol.ToString().Split('.').Last();

            if (name.Length > 0 && (name[1] == 'I' & name[0] == 'I'))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, symbol.Locations[0], name));
            }
        }
    }
}