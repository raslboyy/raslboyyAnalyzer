using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzerTemplate
{
    internal class Collector : CSharpSyntaxWalker
    {
        public ICollection<IfStatementSyntax> Collection { get; } = new List<IfStatementSyntax>();

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            Collection.Add(node);
        }
    }
}
