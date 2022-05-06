using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzerTemplate
{
    internal interface IPossibleToTransform
    {
        bool IsPossibleToTransform(IfStatementSyntax node, SemanticModel model);
    }
}
