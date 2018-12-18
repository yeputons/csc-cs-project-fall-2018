using SyntaxTree.Types;

namespace SyntaxTree.Nodes
{
    public interface IExpression : INode
    {
		IType EvaluationType { get; }
    }
}