using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalyzerTemplate
{
    internal class PossibleToTransform1 : IPossibleToTransform
    {
        public bool IsPossibleToTransform(IfStatementSyntax node, SemanticModel model)
        {
            List<ISymbol> usedVariables = new List<ISymbol>();
            foreach (var variable in node.Condition.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .ToList())
            {
                usedVariables.Add(model.GetSymbolInfo(variable).Symbol);
            }

            var parent = node.Parent;

            foreach (SyntaxNode child in parent.ChildNodes())
            {

                if (child.IsKind(SyntaxKind.IfStatement))
                {
                    break;
                }
                else
                {
                    if (IsIntersect(
                        GetAllVariableSymbols(child, model).ToList(), usedVariables)
                        )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static IEnumerable<ISymbol> GetAllVariableSymbols(SyntaxNode root, SemanticModel model)
        {
            var noDuplicates = new HashSet<ISymbol>();

            foreach (var node in root.DescendantNodesAndSelf())
            {
                switch (node.Kind())
                {
                    case SyntaxKind.IdentifierName:
                        ISymbol symbol = model.GetSymbolInfo(node).Symbol;

                        if (symbol != null && noDuplicates.Add(symbol))
                        {
                            yield return symbol;
                        }
                        break;
                }
            }
        }
        public static bool IsIntersect(IList<ISymbol> collect1, IList<ISymbol> collect2)
        {
            var variables = new HashSet<ISymbol>();
            for (int i = 0; i < collect1.Count(); i++)
                variables.Add(collect1[i]);
            for (int i = 0; i < collect2.Count(); i++)
                if (variables.Contains(collect2[i]))
                    return true;
            return false;
        }
    }
}
