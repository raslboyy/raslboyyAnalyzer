using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerTemplate
{
    internal class Case3 : ICheck
    {
        public bool Check(SemanticModel model, BinaryExpressionSyntax expression)
        {
            var left = expression.Left;
            var right = expression.Right;
            var leftType = model.GetTypeInfo(left).Type;
            var rightType = model.GetTypeInfo(right).Type;

            if (FindOperator(model, leftType, rightType))
                return false;
            if (FindOperator(model, rightType, leftType))
                return false;

            return true;
        }

        private bool FindOperator(SemanticModel model, ITypeSymbol left, ITypeSymbol right)
        {
            if (left.DeclaringSyntaxReferences == null || left.DeclaringSyntaxReferences.FirstOrDefault() == null)
                return false;
            var declaration = (left.DeclaringSyntaxReferences.FirstOrDefault()
                    .GetSyntax() as ClassDeclarationSyntax);
            var allOperators = declaration.Members.OfType<OperatorDeclarationSyntax>().ToList();
            if (allOperators.Where(m =>
            left.ToString().Contains(m.ParameterList.Parameters[0].Type.ToString()) && right.ToString().Contains(m.ParameterList.Parameters[1].Type.ToString())
            || left.ToString().Contains(m.ParameterList.Parameters[1].Type.ToString()) && right.ToString().Contains(m.ParameterList.Parameters[0].Type.ToString())).Count() != 0)
                return true;
            return FindOperator(model, left.BaseType, right);
        }

        private bool IsChildOf(ITypeSymbol parent, ITypeSymbol child)
        {
            if (child == null)
                return false;
            if (SymbolEqualityComparer.Default.Equals(parent, child))
                return true;
            return IsChildOf(parent, child.BaseType);
        }
    }
}
