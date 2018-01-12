using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using ItcLabAnalyzer;

namespace ItcLabAnalyzer.Test
{
    [TestClass]
    public class DisposablesTests : CodeFixVerifier
    {
        [TestMethod]
        public void TestRewritesVariableDeclarationWithUsing()
        {
            var testCode = @"
using System;
using System.IO;

namespace ConsoleApp1
{
    class DisposableTest
    {
        void Foo()
        {
            var m = new MemoryStream();
            // other statements
        }
    }
}
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "DisposablesAnalyzer",
                Message = "Missing call to Dispose()",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 21)
                        }
            };

            VerifyCSharpDiagnostic(testCode, expectedDiagnostic);

            var expectedCode = @"
using System;
using System.IO;

namespace ConsoleApp1
{
    class DisposableTest
    {
        void Foo()
        {
            using (var m = new MemoryStream())
            {
                // other statements
            }
        }
    }
}
";

            VerifyCSharpFix(testCode, expectedCode);
        }

        [TestMethod]
        public void TestRewritesExpressionWithUsing()
        {
            var testCode = @"
using System;
using System.IO;

namespace ConsoleApp1
{
    class DisposableTest
    {
        void Foo()
        {
            new MemoryStream();
            // other statements
        }
    }
}
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "DisposablesAnalyzer",
                Message = "Missing call to Dispose()",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 13)
                        }
            };

            VerifyCSharpDiagnostic(testCode, expectedDiagnostic);

            var expectedCode = @"
using System;
using System.IO;

namespace ConsoleApp1
{
    class DisposableTest
    {
        void Foo()
        {
            using (var disposable = new MemoryStream())
            {
                // other statements
            }
        }
    }
}
";

            VerifyCSharpFix(testCode, expectedCode);
        }

        [TestMethod]
        public void TestIgnoresAlreadyDisposed()
        {
            var testCode = @"
using System;
using System.IO;

namespace ConsoleApp1
{
    class DisposableTest
    {
        void Foo()
        {
            var m = new MemoryStream();
            m.Dispose();
        }
    }
}
";

            VerifyCSharpDiagnostic(testCode);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ItcLabAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ItcLabAnalyzerAnalyzer();
        }
    }
}
