﻿namespace IDisposableAnalyzers.Test.IDISP011DontReturnDisposedTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal partial class ValidCode
    {
        public class Recursion
        {
            [Test]
            public void IgnoresWhenDisposingRecursiveProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class C : IDisposable
    {
        public IDisposable RecursiveProperty => RecursiveProperty;

        public void Dispose()
        {
            this.RecursiveProperty.Dispose();
        }
    }
}";
                RoslynAssert.Valid(Analyzer, testCode);
            }

            [Test]
            public void IgnoresWhenNotDisposingRecursiveProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class C : IDisposable
    {
        public IDisposable RecursiveProperty => RecursiveProperty;

        public void Dispose()
        {
        }
    }
}";
                RoslynAssert.Valid(Analyzer, testCode);
            }

            [Test]
            public void IgnoresWhenDisposingFieldAssignedWithRecursiveProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class C : IDisposable
    {
        private IDisposable disposable;

        public C()
        {
            this.disposable = this.RecursiveProperty;
        }

        public IDisposable RecursiveProperty => RecursiveProperty;

        public void Dispose()
        {
            this.disposable.Dispose();
        }
    }
}";
                RoslynAssert.Valid(Analyzer, testCode);
            }

            [Test]
            public void IgnoresWhenNotDisposingFieldAssignedWithRecursiveProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class C : IDisposable
    {
        private IDisposable disposable;

        public C()
        {
            this.disposable = this.RecursiveProperty;
        }

        public IDisposable RecursiveProperty => RecursiveProperty;

        public void Dispose()
        {
        }
    }
}";
                RoslynAssert.Valid(Analyzer, testCode);
            }

            [Test]
            public void IgnoresWhenDisposingRecursiveMethod()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class C
    {
        public IDisposable RecursiveMethod() => RecursiveMethod();

        public void Dispose()
        {
            this.RecursiveMethod().Dispose();
        }
    }
}";
                RoslynAssert.Valid(Analyzer, testCode);
            }
        }
    }
}
