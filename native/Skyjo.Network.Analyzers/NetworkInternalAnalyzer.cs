using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Skyjo.Network.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NetworkInternalAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NET001";

    private static readonly DiagnosticDescriptor Rule = new(
#pragma warning disable RS2008
        DiagnosticId,
#pragma warning restore RS2008
        title: "Direct call not allowed",
        messageFormat: "'{0}' is marked [NetworkInternal] and cannot be called directly",
        category: "Network",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var symbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (symbol is null)
            return;

        if (!HasNetworkInternalAttribute(symbol))
            return;

        // Allow calls from the same assembly as the [NetworkInternal] method
        var callerMethod = GetContainingMethod(invocation, context.SemanticModel);
        if (callerMethod is not null &&
            SymbolEqualityComparer.Default.Equals(
                callerMethod.ContainingAssembly, symbol.ContainingAssembly))
            return;

        // Allow calls from within a [Template] method (Metalama aspect templates)
        if (callerMethod is not null && HasTemplateAttribute(callerMethod))
            return;

        context.ReportDiagnostic(
            Diagnostic.Create(Rule, invocation.GetLocation(), symbol.Name));
    }

    private static bool HasNetworkInternalAttribute(IMethodSymbol method)
    {
        foreach (var attr in method.GetAttributes())
        {
            if (attr.AttributeClass?.Name == "NetworkInternalAttribute")
                return true;
        }
        return false;
    }

    private static bool HasTemplateAttribute(IMethodSymbol method)
    {
        var current = (IMethodSymbol?)method;
        while (current is not null)
        {
            foreach (var attr in current.GetAttributes())
            {
                if (attr.AttributeClass?.Name == "TemplateAttribute")
                    return true;
            }
            current = current.OverriddenMethod;
        }
        return false;
    }

    private static IMethodSymbol? GetContainingMethod(SyntaxNode node, SemanticModel model)
    {
        var methodDecl = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDecl is not null)
            return model.GetDeclaredSymbol(methodDecl);

        var accessorDecl = node.FirstAncestorOrSelf<AccessorDeclarationSyntax>();
        if (accessorDecl is not null)
            return model.GetDeclaredSymbol(accessorDecl);

        return null;
    }
}
