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
    public class VariableDefinitionCodeFixProvider
    {
        public const string Title = "Long variable name";

        public async Task<Document> SeparateVariablesAsync(Document document, VariableDeclaratorSyntax name, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var newIdenfiter = "";
            string nameString = name.Identifier.Text;

            var regexList = new List<string>();
            Regex regex;

            if (nameString.Contains("_"))
            {
                regex = new Regex("[^_]+");
                regexList = (from Match m in regex.Matches(nameString) select m.Value).ToList();

                foreach (var n in regexList)
                {
                    newIdenfiter += n[0];
                }
            }
            else if (Regex.Match(nameString, "[A-Z]").Success)
            {
                regex = new Regex("[A-Z0-9]");
                regexList = (from Match m in regex.Matches(nameString) select m.Value).ToList();

                foreach (var n in regexList)
                {
                    newIdenfiter += n;
                }
            }
            else
                newIdenfiter = name.Identifier.Text;

            var updatedSyntaxTree =
        syntaxTree.GetRoot().ReplaceNode(name,
            name.WithIdentifier(SyntaxFactory.ParseToken(newIdenfiter.ToLower())));
            

            
            return document.WithSyntaxRoot(updatedSyntaxTree);
            
        }
    }
}
