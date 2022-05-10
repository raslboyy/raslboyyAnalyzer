using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerTemplate
{
    internal class Case1 : ICheck
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

            if (leftEqualityMethods.Count() == 0 && rightEqualityMethods.Count() == 0)
                return true;
            return false;
        }
    }
}
