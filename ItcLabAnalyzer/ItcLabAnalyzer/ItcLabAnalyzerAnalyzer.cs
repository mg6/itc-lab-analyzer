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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ItcLabAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private PathCombineAnalyzer CombineAnalyzer = new PathCombineAnalyzer();

        private VariableDefinitionAnalyzer VariableAnalyzer = new VariableDefinitionAnalyzer();

        private DisposablesAnalyzer DisposablesAnalyzer = new DisposablesAnalyzer();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DisposablesAnalyzer.Rule,
                    PathCombineAnalyzer.Rule,
                    VariableDefinitionAnalyzer.Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(DisposablesAnalyzer.Analyze, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(CombineAnalyzer.AnalyzeStringPath, SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(VariableAnalyzer.AnalyzeVariableDefinition, SyntaxKind.VariableDeclarator);
        }
    }
}
