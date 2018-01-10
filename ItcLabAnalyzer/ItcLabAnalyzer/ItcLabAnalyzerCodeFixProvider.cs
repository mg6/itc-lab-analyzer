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
        private const string title = "Change to Path.Combine";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ItcLabAnalyzerAnalyzer.DiagnosticId); }
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
                CodeAction.Create(title, c =>
                MakePathCombineAsync(context.Document, exprDec, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> MakePathCombineAsync(Document document, LiteralExpressionSyntax litExp, CancellationToken cancellationToken)
        {
            
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var list = new List<string>();
            var checkParent = litExp.Parent;

            if (checkParent as BinaryExpressionSyntax != null) // if Parent was a + expression
            {
                while (true)
                {
                    if (checkParent.Parent as BinaryExpressionSyntax != null)
                        checkParent = checkParent.Parent;
                    else break;
                }
                checkCon(checkParent as BinaryExpressionSyntax, ref list);
            }
            else
            {
                var regexList = variables(litExp.Token.ValueText);
                for(int i=regexList.Count-1; i>=0; i--)
                {
                    list.Add(String.Format("\"{0}\"", regexList.ElementAt(i)));
                }
            }

            string newPath = list.ElementAt(0);
            for(int i=1; i<list.Count; i++)
            {
                newPath = String.Format("Path.Combine({0},{1})", list.ElementAt(i), newPath);
            }

            if (checkParent as BinaryExpressionSyntax != null) // if Parent was a + expression
            {
                var updatedSyntaxTree =
                    syntaxTree.GetRoot().ReplaceNode(checkParent, SyntaxFactory.ParseExpression(newPath));
                return document.WithSyntaxRoot(updatedSyntaxTree);
            }
            else
            {
                var updatedSyntaxTree =
                syntaxTree.GetRoot().ReplaceNode(litExp, SyntaxFactory.ParseExpression(newPath));
                return document.WithSyntaxRoot(updatedSyntaxTree);
            }

            
        }

        private void checkCon(BinaryExpressionSyntax add, ref List<string> list)
        {
            if (add != null)
            {  
                if (add.Right as IdentifierNameSyntax != null) // if variable
                {
                    var ar = (IdentifierNameSyntax)add.Right;
                    list.Add(ar.Identifier.Text);
                }
                else if (add.Right as LiteralExpressionSyntax != null) //if string
                {
                    var ar = (LiteralExpressionSyntax)add.Right;
                    var regexList = variables(ar.Token.ValueText);
                    for (int i = regexList.Count-1; i >= 0; i--)
                    {
                        list.Add(String.Format("\"{0}\"", regexList.ElementAt(i)));
                    }
                }//if another +
                else if (add.Right as BinaryExpressionSyntax != null) checkCon((BinaryExpressionSyntax)add.Right, ref list);

                if (add.Left as IdentifierNameSyntax != null) // if variable
                {
                    var al = (IdentifierNameSyntax)add.Left;
                    list.Add(al.Identifier.Text);
                }
                else if (add.Left as LiteralExpressionSyntax != null) //if string
                {
                    var al = (LiteralExpressionSyntax)add.Left;
                    var regexList = variables(al.Token.ValueText);
                    for (int i = regexList.Count-1; i >= 0; i--)
                    {
                        list.Add(String.Format("\"{0}\"", regexList.ElementAt(i)));
                    }
                }//if another +
                else if (add.Left as BinaryExpressionSyntax != null) checkCon((BinaryExpressionSyntax)add.Left, ref list);
            }
        }

        private List<string> variables (string path)
        {
            var regexList = new List<string>();
            var list = new List<string>();
            Regex regex;

            if (path.Contains("@\""))
            {
                regex = new Regex("[^\\]+");
                regexList = (from Match m in regex.Matches(path.Substring(1)) select m.Value).ToList();
            }
            else
            {
                regex = new Regex("[^\\\\]+");
                regexList = (from Match m in regex.Matches(path) select m.Value).ToList();
            }
            return regexList;
        }
    }
}
