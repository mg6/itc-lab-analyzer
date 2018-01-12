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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ItcLabAnalyzer
{
    public class DisposablesCodeFixProvider
    {
        public static readonly string DiagnosticId = "DisposablesAnalyzer";
        public static readonly string Title = "Disposable Analyzer";

        public async Task<Document> MakeDisposablesAsync(Document document, ObjectCreationExpressionSyntax expression, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);

            var enclosingDeclaration = expression.FirstAncestorOrSelf<VariableDeclarationSyntax>();
            var enclosingStatement = expression.FirstAncestorOrSelf<StatementSyntax>();
            var enclosingBlock = expression.FirstAncestorOrSelf<BlockSyntax>();

            if (enclosingBlock == null)
            {
                return document;
            }

            var newDeclaration = enclosingDeclaration;
            if (enclosingDeclaration == null)
            {
                // Thank God there exists https://github.com/KirillOsenkov/RoslynQuoter !!
                newDeclaration =
                    VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                Identifier("disposable"))
                            .WithInitializer(
                                EqualsValueClause(
                                    expression))));
            }

            var usingStatement = UsingStatement(Block())
                .WithDeclaration(newDeclaration);

            var newStatements = enclosingBlock.Statements
                .Replace(enclosingStatement, usingStatement);

            var newBlock = enclosingBlock.WithStatements(newStatements);

            var oldRoot = syntaxTree.GetRoot();
            var newRoot = oldRoot.ReplaceNode(enclosingBlock, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
