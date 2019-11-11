namespace IDisposableAnalyzers.Test.IDISP008DontMixInjectedAndCreatedForMemberTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class Diagnostics
    {
        private static readonly AssignmentAnalyzer Analyzer = new AssignmentAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("IDISP008");

        private const string Disposable = @"
namespace RoslynSandbox
{
    using System;

    public class Disposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}";

        [Test]
        public static void PublicMethodRefParameter()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    public class C
    {
        public bool TryGetStream(ref Stream stream)
        {
            ↓stream = File.OpenRead(string.Empty);
            return true;
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, testCode);
        }
    }
}
