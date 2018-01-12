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
using Microsoft.CodeAnalysis.Formatting;

namespace ItcLabAnalyzer
{
    public class VariableDefinitionCodeFixProvider
    {
        public const string Title = "Long variable name";

        public async Task<Document> SeparateVariablesAsync(Document document, VariableDeclaratorSyntax name, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var newIdenfiter = name.Identifier.Text;
            string nameString = name.Identifier.Text;

            var regexList = new List<string>();
            Regex regex;

            if (nameString.Contains("_"))
            {
                regex = new Regex("[^_]+");
                regexList = (from Match m in regex.Matches(nameString)
                             select (int.TryParse(m.Value, out var _) ? m.Value : m.Value.Substring(0, 1))).ToList();
                newIdenfiter = String.Join("", regexList);
            }
            else if (Regex.Match(nameString, "[A-Z]").Success)
            {
                regex = new Regex("[A-Z0-9]");
                regexList = (from Match m in regex.Matches(nameString) select m.Value).ToList();
                newIdenfiter = String.Join("", regexList);
            }

            var oldRoot = syntaxTree.GetRoot();
            var newName = name.WithIdentifier(SyntaxFactory.ParseToken(newIdenfiter.ToLower()))
                    .WithLeadingTrivia(name.GetLeadingTrivia())
                    .WithTrailingTrivia(name.GetTrailingTrivia())
                    .WithAdditionalAnnotations(Formatter.Annotation);
            var newRoot = oldRoot.ReplaceNode(name, newName);

            return document.WithSyntaxRoot(newRoot);

        }
    }
}
