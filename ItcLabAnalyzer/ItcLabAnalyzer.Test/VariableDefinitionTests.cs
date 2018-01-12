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
        public void TestPascalCaseVariableName()
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
        public void TestSnakeCaseVariableName()
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
        public void TestSnakeCaseVariableNameWithNumbers()
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
                int bardzo_bardzo_22_długa_nazwa_to_jest_33;
                int bardzo_bardzo_44długa_nazwa_to_jest55;
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
        public void TestPascalCaseVariableNameWithNumbers()
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
                int BardzoBardzo55DługaNazwaToJest;
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
                int bb55dntj;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestLowerCaseVariableName()
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

        [TestMethod]
        public void TestManySnakeCaseVariableNames()
        {
            var test = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bardzo_bardzo_długa_nazwa_to_jest,
                    inna_bardzo_bardzo_długa_nazwa_to_jest;
            }
        }
    }
";

            var fixtest = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj,
                    ibbdntj;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestManyPascalCaseVariableNames()
        {
            var test = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int BardzoBardzoDługaNazwaToJest,
                    InnaBardzoBardzoDługaNazwaToJest;
            }
        }
    }
";

            var fixtest = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj,
                    ibbdntj;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestPascalCaseVariableNameWithInitializer()
        {
            var test = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int BardzoBardzoDługaNazwaToJest = 5 + 6;
            }
        }
    }
";

            var fixtest = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj = 5 + 6;
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestSnakeCaseVariableNameWithInitializer()
        {
            var test = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bardzo_bardzo_długa_nazwa_to_jest = 5 + 6;
            }
        }
    }
";

            var fixtest = @"
    using System;
    namespace ConsoleApplication1
    {
        class TypeName
        {           
            void Method() 
            {
                int bbdntj = 5 + 6;
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
