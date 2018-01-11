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
    public class VariableDefinitionAnalyzer
    {
        public const string DiagnosticId = "VarDefAnalyzer";

        private static readonly LocalizableString Title = "Long variable name";
        private static readonly LocalizableString MessageFormat = "Variable name is long";
        private static readonly LocalizableString Description = "Variables shouldn't have long names";
        private const string Category = "Naming";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public void AnalyzeVariableDefinition(SyntaxNodeAnalysisContext context)
        {
            var name = (VariableDeclaratorSyntax)context.Node;

            if(name.Identifier.Text.Length >=15)
            {
                var diagnostic = Diagnostic.Create(Rule, name.GetLocation(), name);
                            context.ReportDiagnostic(diagnostic);
            }
                
        }
    }
}
