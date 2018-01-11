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
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethod2()
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
                string s = ""aa\\bb"";
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
                string s = Path.Combine(""aa"",""bb"");
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestMethod3()
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
                string s = @""aa\bb"";
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
                string s = Path.Combine(""aa"",""bb"");
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestMethod4()
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
                string s = ""aa\\bb\\cc"";
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
                string s = Path.Combine(""aa"",Path.Combine(""bb"",""cc""));
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestMethod5()
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
                string s = ""aa\\bb\\"";
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
                string s = Path.Combine(""aa"",""bb"");
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestMethod6()
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
                string n = ""aaa"";
                string s = ""aa\\bb"" + n;
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
                string n = ""aaa"";
                string s = Path.Combine(""aa"",Path.Combine(""bb"",n));
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestMethod7()
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
                string n = ""aaa"";
                string s = ""aa\\bb"" + n + ""wz"";
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
                string n = ""aaa"";
                string s = Path.Combine(""aa"",Path.Combine(""bb"",Path.Combine(n,""wz"")));
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
