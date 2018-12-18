using System;
using System.Collections.Generic;
using System.Text;
using SyntaxTree.Types;

namespace SyntaxTree.Nodes
{
	public class Int32Constant : EmptyNode, IExpression
	{
		public Int32Constant(int value)
		{
			Value = value;
		}

		public int Value { get; }
		public IType EvaluationType => SInt32.Instance;
	}

	public class Int64Constant : EmptyNode, IExpression
	{
		public Int64Constant(long value)
		{
			Value = value;
		}

		public long Value { get; }
		public IType EvaluationType => SInt64.Instance;
	}

	public class CharConstant : EmptyNode, IExpression
	{
		public CharConstant(char value)
		{
			Value = value;
		}

		public char Value { get; }
		public IType EvaluationType => SChar.Instance;
	}

	public class BoolConstant : EmptyNode, IExpression
	{
		public BoolConstant(bool value)
		{
			Value = value;
		}

		public bool Value { get; }
		public IType EvaluationType => SBoolean.Instance;
	}
}
