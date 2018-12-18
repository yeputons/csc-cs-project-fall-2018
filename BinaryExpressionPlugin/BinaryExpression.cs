using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxTree.Nodes;
using SyntaxTree.Types;

namespace BinaryExpressionPlugin
{
    public class BinaryExpression : IExpression
    {
	    public BinaryExpression(IExpression left, OperationType operation, IExpression right)
	    {
		    Left = left ?? throw new ArgumentException("Left should be non-null");
		    Operation = operation;
		    Right = right ?? throw new ArgumentException("Right should be non-null");
		    Children = new[] {Left, Right};
		    EvaluationType = GetType(Left.EvaluationType, Operation, Right.EvaluationType);
	    }

		public enum OperationType
	    {
			Plus, Minus, Multiply, Divide,
			Eq
	    }

	    private static IType GetType(IType left, OperationType operation, IType right)
	    {
		    switch (operation)
		    {
			    case OperationType.Plus:
			    case OperationType.Minus:
			    case OperationType.Multiply:
			    case OperationType.Divide:
				    if (left == SInt32.Instance && right == SInt32.Instance)
					    return SInt32.Instance;
				    else if (left == SInt32.Instance && right == SInt64.Instance)
					    return SInt64.Instance;
				    else if (left == SInt64.Instance && right == SInt32.Instance)
					    return SInt64.Instance;
				    else if (left == SInt64.Instance && right == SInt64.Instance)
					    return SInt64.Instance;
				    break;
			    case OperationType.Eq:
				    if (left == right)
					    return SBoolean.Instance;
				    else if (left == SInt32.Instance && right == SInt64.Instance)
					    return SBoolean.Instance;
				    else if (left == SInt64.Instance && right == SInt32.Instance)
					    return SBoolean.Instance;
				    break;
		    }
		    throw new ArgumentException($"Cannot apply {operation} to {left} and {right}");
	    }

		public IExpression Left { get; }
		public OperationType Operation { get; }
	    public IExpression Right { get; }

		public IReadOnlyList<INode> Children { get; }
	    public IType EvaluationType { get; }
    }
}
