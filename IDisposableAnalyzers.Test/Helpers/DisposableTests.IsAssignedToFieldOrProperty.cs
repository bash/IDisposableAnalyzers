namespace IDisposableAnalyzers.Test.Helpers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class DisposableTests
    {
        public class IsAssignedToFieldOrProperty
        {
            [Test]
            public void WhenNotUsed()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        internal C(IDisposable disposable)
        {
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true, LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(false, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void AssigningLocal()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        internal C(IDisposable disposable)
        {
            var temp = disposable;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true,  LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(false, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void FieldAssignedInCtor()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        private IDisposable disposable;

        internal C(IDisposable disposable)
        {
            this.disposable = disposable;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true,  LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(true, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void ArrayFieldAssignedInCtor()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        private IDisposable[] disposables = new IDisposable[1];

        internal C(IDisposable disposable)
        {
            this.disposables[0] = disposable;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true, LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(true, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void FieldAssignedInCtorCallingInitialize()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        private IDisposable disposable;

        internal C(IDisposable disposable)
        {
            this.Initialize(disposable);
        }

        private void Initialize(IDisposable arg)
        {
            this.disposable = arg;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true, LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(true, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void FieldAssignedInCtorViaLocal()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        private IDisposable disposable;

        internal C(IDisposable disposable)
        {
            var temp = disposable;
            this.disposable = temp;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true, LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(true, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }

            [Test]
            public void PropertyAssignedInCtor()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    internal class C
    {
        internal C(IDisposable disposable)
        {
            this.Disposable = disposable;
        }

        public IDisposable Disposable { get; }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindParameter("IDisposable disposable");
                var symbol = semanticModel.GetDeclaredSymbol(value, CancellationToken.None);
                Assert.AreEqual(true, LocalOrParameter.TryCreate(symbol, out var localOrParameter));
                Assert.AreEqual(true, Disposable.IsAssignedToFieldOrProperty(localOrParameter, semanticModel, CancellationToken.None, null));
            }
        }
    }
}
