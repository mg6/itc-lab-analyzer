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

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    PathCombineAnalyzer.DiagnosticId);
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var exprDec = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
            context.RegisterCodeFix(
                CodeAction.Create(PathCombineCodeFixProvider.Title,
                    c => PathCombineCodeFixProvider.MakePathCombineAsync(context.Document, exprDec, c),
                    equivalenceKey: PathCombineCodeFixProvider.Title),
                diagnostic);
        }
    }
}
