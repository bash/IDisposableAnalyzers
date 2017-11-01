﻿namespace IDisposableAnalyzers.Test.IDISP001DisposeCreatedTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal partial class CodeFix
    {
        internal class AddUsing
        {
            [Test]
            public void AddUsingForLocal()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓var stream = File.OpenRead(string.Empty);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (var stream = File.OpenRead(string.Empty))
            {
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
            }

            [Test]
            public void AddUsingForIgnored()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓File.OpenRead(string.Empty);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (File.OpenRead(string.Empty))
            {
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
            }

            [Test]
            public void AddUsingForLocalOneStatementAfter()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓var stream = File.OpenRead(string.Empty);
            var i = 1;
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (var stream = File.OpenRead(string.Empty))
            {
                var i = 1;
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
            }

            [Test]
            public void AddUsingForIgnoredOneStatementAfter()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓File.OpenRead(string.Empty);
            var i = 1;
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (File.OpenRead(string.Empty))
            {
                var i = 1;
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
            }

            [Test]
            public void AddUsingForLocalManyStatements()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓var stream = File.OpenRead(string.Empty);
            var a = 1;
            var b = 1;
            if (a == b)
            {
                var c = 2;
            }
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (var stream = File.OpenRead(string.Empty))
            {
                var a = 1;
                var b = 1;
                if (a == b)
                {
                    var c = 2;
                }
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP001DisposeCreated, AddUsingCodeFixProvider>(testCode, fixedCode);
            }

            [Test]
            public void AddUsingForIgnoredManyStatements()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            ↓File.OpenRead(string.Empty);
            var a = 1;
            var b = 1;
            if (a == b)
            {
                var c = 2;
            }
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class Foo
    {
        public void Meh()
        {
            using (File.OpenRead(string.Empty))
            {
                var a = 1;
                var b = 1;
                if (a == b)
                {
                    var c = 2;
                }
            }
        }
    }
}";
                AnalyzerAssert.CodeFix<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
                AnalyzerAssert.FixAll<IDISP004DontIgnoreReturnValueOfTypeIDisposable, AddUsingCodeFixProvider>(testCode, fixedCode);
            }
        }
    }
}