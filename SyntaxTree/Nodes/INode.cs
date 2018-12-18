using System.Collections.Generic;

namespace SyntaxTree.Nodes
{
    public interface INode
    {
		IReadOnlyList<INode> Children { get; }
    }

	public abstract class EmptyNode : INode
	{
		protected EmptyNode()
		{
			Children = new List<INode>();
		}

		public IReadOnlyList<INode> Children { get; }
	}
}