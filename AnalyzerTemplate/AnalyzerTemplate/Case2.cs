using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysisApp1
{
    internal class Case2 : ICheck
    {
        public bool Check(SemanticModel model, BinaryExpressionSyntax expression)
        {
            const string equalityMethodName = "op_Equality";
            var left = expression.Left;
            var right = expression.Right;
            var leftType = model.GetTypeInfo(left).Type;
            var rightType = model.GetTypeInfo(right).Type;
            var leftEqualityMethods = leftType.GetMembers().OfType<IMethodSymbol>().Where(m => m.Name == equalityMethodName);
            var rightEqualityMethods = rightType.GetMembers().OfType<IMethodSymbol>().Where(m => m.Name == equalityMethodName);

            if (new Case1().Check(model, expression))
                return true;

            if (leftEqualityMethods.Where(m =>
            SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, leftType) && SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, rightType)
            || SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, rightType) && SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, leftType)).Count() != 0)
                return false;

            if (rightEqualityMethods.Where(m =>
            SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, leftType) && SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, rightType)
            || SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, rightType) && SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, leftType)).Count() != 0)
                return false;

            return true;
        }
    }
}
