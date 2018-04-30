namespace IDisposableAnalyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ObjectCreationExt
    {
        internal static bool Creates(this ObjectCreationExpressionSyntax creation, ConstructorDeclarationSyntax ctor, ReturnValueSearch search, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var created = semanticModel.GetSymbolSafe(creation, cancellationToken);
            var ctorSymbol = semanticModel.GetDeclaredSymbolSafe(ctor, cancellationToken);
            if (SymbolComparer.Equals(ctorSymbol, created))
            {
                return true;
            }

            return search == ReturnValueSearch.Recursive &&
                   Constructor.IsRunBefore(created, ctorSymbol, semanticModel, cancellationToken);
        }
    }
}
