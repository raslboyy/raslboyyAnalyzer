using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTemplate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerTemplateAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AnalyzerTemplate";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            /*context.RegisterSyntaxTreeAction(syntaxTreeContext =>
            {
                // Iterate through all statements in the tree
                var tree = syntaxTreeContext.Tree;
                var root = tree.GetRoot(syntaxTreeContext.CancellationToken);
                var compilation = CSharpCompilation.Create("")
                    .AddReferences(MetadataReference.CreateFromFile(
                    typeof(string).Assembly.Location))
                    .AddSyntaxTrees(tree);
                var model = compilation.GetSemanticModel(tree);

                var collector = new ElseWalker();
                collector.Visit(root);
                foreach (var directive in collector.Collection)
                {
                    if (directive.Parent.IsKind(SyntaxKind.ElseClause))
                        continue;
                    if (!new PossibleToTransform1().IsPossibleToTransform(directive, semanticModel))
                        continue;
                    var diagnostic = Diagnostic.Create(Rule, directive.GetFirstToken().GetLocation());
                    syntaxTreeContext.ReportDiagnostic(diagnostic);
                }

            });*/

            context.RegisterSemanticModelAction(semanticModelAnalysisContext =>
            {
                SemanticModel semanticModel = semanticModelAnalysisContext.SemanticModel;
                SyntaxNode root = semanticModel.SyntaxTree.GetRoot();

                var collector = new ElseWalker();
                collector.Visit(root);
                foreach (var directive in collector.Collection)
                {
                    /*if (!new PossibleToTransform1().IsPossibleToTransform(directive, semanticModel))
                        continue;*/

                    var diagnostic = Diagnostic.Create(Rule, directive.GetFirstToken().GetLocation());
                    semanticModelAnalysisContext.ReportDiagnostic(diagnostic);
                }
            });
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
