using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerTemplateCodeFixProvider)), Shared]
    public class AnalyzerTemplateCodeFixProvider : CodeFixProvider
    {
        private const string title = "Merge";
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(AnalyzerTemplateAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            var tree = await context.Document.GetSyntaxTreeAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Why don't work???
            /*var node = root.FindNode(diagnostic.Location.SourceSpan);
            if (!new PossibleToTransform1().IsPossibleToTransform((IfStatementSyntax)node, model))
            {
                return;
            }*/

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                title: title,
                createChangedDocument: c => MergeInnerIfAsync(context.Document, diagnostic, root, model),
                equivalenceKey: title),
            diagnostic);
        }

        Task<Document> MergeInnerIfAsync(Document document, Diagnostic diagnostic, SyntaxNode root, SemanticModel model)
        {
            var oldNode = root.FindNode(diagnostic.Location.SourceSpan).Parent.FirstAncestorOrSelf<StatementSyntax>();

            List<SyntaxNode> before = new List<SyntaxNode>();
            List<SyntaxNode> after = new List<SyntaxNode>();
            IfStatementSyntax newNode = null;

            foreach (SyntaxNode child in oldNode.ChildNodes())
            {
                if (child.IsKind(SyntaxKind.IfStatement))
                {
                    newNode = (IfStatementSyntax)child;
                    continue;
                }

                if (newNode == null)
                {
                    before.Add(child);
                }
                else
                {
                    after.Add(child);
                }
            }

            foreach (IfStatementSyntax node in newNode.DescendantNodesAndSelf()
                .Where(
                node => node.Span.End == newNode.Span.End
                ))
            {
                List<SyntaxNode> body = new List<SyntaxNode>();
                body.AddRange(before);
                body.AddRange(node.Statement.ChildNodes());
                body.AddRange(after);

                var wrapperBody = SyntaxFactory.Block(SyntaxFactory.List(body));
                newNode = newNode.ReplaceNode(node.Statement, wrapperBody);
            }

            foreach (ElseClauseSyntax node in newNode.DescendantNodesAndSelf()
                .Where(
                node => node.Span.End == newNode.Span.End && node.IsKind(SyntaxKind.ElseClause)
                ))
            {
                List<SyntaxNode> body = new List<SyntaxNode>();
                body.AddRange(before);
                body.AddRange(node.Statement.ChildNodes());
                body.AddRange(after);

                var wrapperBody = SyntaxFactory.Block(SyntaxFactory.List(body));
                newNode = newNode.ReplaceNode(node.Statement, wrapperBody);
            }

            var newRoot = root.ReplaceNode(oldNode, newNode);
            return Task.FromResult(document.WithSyntaxRoot(newRoot.NormalizeWhitespace().SyntaxTree.GetRoot()));
        }
    }
}
