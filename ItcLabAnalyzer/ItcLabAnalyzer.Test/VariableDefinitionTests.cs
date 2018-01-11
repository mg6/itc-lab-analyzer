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
    public class VariableDefinitionTests : CodeFixVerifier
    {
        [TestMethod]
        public void TestNameWithUpperLetters()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int BardzoBardzoDługaNazwaToJest;
            }
        }
    }
";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestNameWithUnderScore()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bardzo_bardzo_długa_nazwa_to_jest;
            }
        }
    }
";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestNameWithUpperLettersAndNumbers()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int BardzoBardzoDługaNazwaToJest33;
            }
        }
    }
";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj33;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestNameWithLowerLetters()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int tojestbardzodluganazwa;
            }
        }
    }
";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int tojestbardzodluganazwa;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
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
