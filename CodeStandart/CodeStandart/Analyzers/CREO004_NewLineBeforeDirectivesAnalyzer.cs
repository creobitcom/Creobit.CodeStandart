using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStandart.Utilities;
using System.Linq;

namespace CodeStandart.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO004_NewLineBeforeDirectivesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO004";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode,
            SyntaxKind.IfStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.BreakStatement,
            SyntaxKind.YieldReturnStatement,
            SyntaxKind.YieldBreakStatement,
            SyntaxKind.TryStatement,
            SyntaxKind.ContinueStatement,
            SyntaxKind.SwitchStatement,
            SyntaxKind.ReturnStatement);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (AnalyzerUtility.CheckIfHaveEmptyStringBefore(node) ||
                AnalyzerUtility.CheckIfNodeFirstInBlock(node))
            {
                return;
            }

            if (node.Ancestors().Any(ancestor => ancestor.Kind() == SyntaxKind.SwitchSection))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation(), node.Kind()));
        }
    }
}
