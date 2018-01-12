using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ItcLabAnalyzer
{
    public class DisposablesAnalyzer
    {
        public const string DiagnosticId = "DisposablesAnalyzer";

        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.DisposablesAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.DisposablesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.DisposablesAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            var expression = context.Node as ObjectCreationExpressionSyntax;

            var declarator = expression.Parent.Parent as VariableDeclaratorSyntax;
            var identifier = declarator.Identifier;

            var classType = context.SemanticModel.GetTypeInfo(expression).ConvertedType;
            var implementedInterfaces = classType.AllInterfaces;

            if (implementedInterfaces.Select(e => e.Name).Contains(nameof(IDisposable)))
            {
                var root = context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken);
                var disposeInvocations = root.DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(e => (e.Expression as IdentifierNameSyntax).Identifier.Text == identifier.Text);

                if (!disposeInvocations.Any())
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), context.Node);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
