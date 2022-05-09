using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysisApp1
{
    internal interface ICheck
    {
        bool Check(SemanticModel model, BinaryExpressionSyntax expression);
    }
}
