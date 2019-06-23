namespace IDisposableAnalyzers.Test.IDISP001DisposeCreatedTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public partial class CodeFix
    {
        public class AddUsingDeclaration
        {
            private static readonly DiagnosticAnalyzer Analyzer = new LocalDeclarationAnalyzer();
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(IDISP001DisposeCreated.Descriptor);
            private static readonly CodeFixProvider Fix = new AddUsingFix();

            [Test]
            public void Local()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class C
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

    public sealed class C
    {
        public void Meh()
        {
            using (var stream = File.OpenRead(string.Empty))
            {
            }
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalWithTrivia()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class C
    {
        public void Meh()
        {
            // Some comment
            ↓var stream = File.OpenRead(string.Empty);
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class C
    {
        public void Meh()
        {
            // Some comment
            using (var stream = File.OpenRead(string.Empty))
            {
            }
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalOneStatementAfter()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class C
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

    public sealed class C
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
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalManyStatements()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public sealed class C
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

    public sealed class C
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
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalInLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public class C
    {
        public C()
        {
            Console.CancelKeyPress += (_, __) =>
            {
                ↓var stream = File.OpenRead(string.Empty);
            };
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public class C
    {
        public C()
        {
            Console.CancelKeyPress += (_, __) =>
            {
                using (var stream = File.OpenRead(string.Empty))
                {
                }
            };
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalInSwitchCase()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    class C
    {
        public C(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    break;
                case StringComparison.InvariantCulture:
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    break;
                case StringComparison.Ordinal:
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    ↓var stream = File.OpenRead(string.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    class C
    {
        public C(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    break;
                case StringComparison.InvariantCulture:
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    break;
                case StringComparison.Ordinal:
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    using (var stream = File.OpenRead(string.Empty))
                    {
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void LocalFactoryMethod()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public class C
    {
        public C()
        {
            ↓var stream = Create();

            IDisposable Create()
            {
                return File.OpenRead(string.Empty);
            }
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public class C
    {
        public C()
        {
            using (var stream = Create())
            {
            }

            IDisposable Create()
            {
                return File.OpenRead(string.Empty);
            }
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }
        }
    }
}
