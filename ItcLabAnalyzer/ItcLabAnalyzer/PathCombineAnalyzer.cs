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
    public class PathCombineAnalyzer
    {
        public const string DiagnosticId = "ItcLabAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PathCombineAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PathCombineAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PathCombineAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public void AnalyzeStringPath(SyntaxNodeAnalysisContext context)
        {
            var checkString = (LiteralExpressionSyntax)context.Node;
            if (Regex.Match(checkString.Token.Text, "\\\\").Success || (checkString.Token.Text.Contains("@\"") && checkString.Token.Text.Contains('\\')))
            {
                var diagnostic = Diagnostic.Create(Rule, checkString.GetLocation(), checkString);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
