using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzerTemplate
{
    internal class ElseWalker : CSharpSyntaxWalker
    {
        public List<IfStatementSyntax> Collection = new List<IfStatementSyntax>();

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            if (node.Else != null)
            {
                
                var innerCollector = new Collector();
                innerCollector.Visit(node.Else);
                foreach (var element in innerCollector.Collection)
                {
                    if (element.Parent.Kind() == SyntaxKind.ElseClause)
                        continue;
                    Collection.Add(element);
                }
            }
            base.VisitIfStatement(node);
        }
    }
}
