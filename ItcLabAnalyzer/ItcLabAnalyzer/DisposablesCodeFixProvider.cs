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
    public class DisposablesCodeFixProvider
    {
        public async Task<Document> MakeDisposablesAsync(Document document, LiteralExpressionSyntax litExp, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
