using System;
using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Semantics;

namespace ItcLabAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ItcLabAnalyzerCodeFixProvider)), Shared]
    public class ItcLabAnalyzerCodeFixProvider : CodeFixProvider
    {
        private PathCombineCodeFixProvider PathCombineCodeFixProvider = new PathCombineCodeFixProvider();
        private VariableDefinitionCodeFixProvider VariableDefinitionCodeFixProvider = new VariableDefinitionCodeFixProvider();

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    PathCombineAnalyzer.DiagnosticId,
                    VariableDefinitionAnalyzer.DiagnosticId);
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                switch (diagnostic.Id)
                {
                    case PathCombineAnalyzer.DiagnosticId:
                        var exprDec = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
                        context.RegisterCodeFix(
                            CodeAction.Create(PathCombineCodeFixProvider.Title,
                                c => PathCombineCodeFixProvider.MakePathCombineAsync(context.Document, exprDec, c),
                                equivalenceKey: PathCombineCodeFixProvider.Title),
                            diagnostic);
                        break;
                    case VariableDefinitionAnalyzer.DiagnosticId:
                        var exprDec2 = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
                        context.RegisterCodeFix(
                            CodeAction.Create(VariableDefinitionCodeFixProvider.Title,
                                c => VariableDefinitionCodeFixProvider.SeparateVariablesAsync(context.Document, exprDec2, c),
                                equivalenceKey: VariableDefinitionCodeFixProvider.Title),
                            diagnostic);
                        break;
                }
            }
        }
    }
}
