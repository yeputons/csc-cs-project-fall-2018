using System.Collections.Generic;

namespace SyntaxTree.Nodes
{
    public interface INode
    {
		IReadOnlyList<INode> Children { get; }
    }
}