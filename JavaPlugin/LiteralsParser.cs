using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.github.javaparser.ast.expr;
using SyntaxTree.Nodes;
using SyntaxTree.Types;
using TranspilerInfrastructure;

namespace JavaPlugin
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

	public static class LiteralsParser
	{
		public static readonly TaggedFunction<NodeParsingTag, IntegerLiteralExpr, Int32Constant> ParseInt32Literal = new TaggedFuncWrapper<NodeParsingTag, IntegerLiteralExpr, Int32Constant>(
			literal => new Int32Constant(literal.asInt()));
		public static readonly TaggedFunction<NodeParsingTag, LongLiteralExpr, Int64Constant> ParseInt64Literal = new TaggedFuncWrapper<NodeParsingTag, LongLiteralExpr, Int64Constant>(
			literal => new Int64Constant(literal.asLong()));
		public static readonly TaggedFunction<NodeParsingTag, CharLiteralExpr, CharConstant> ParseCharLiteral = new TaggedFuncWrapper<NodeParsingTag, CharLiteralExpr, CharConstant>(
			literal => new CharConstant(literal.asChar()));
		public static readonly TaggedFunction<NodeParsingTag, BooleanLiteralExpr, BoolConstant> ParseBoolLiteral = new TaggedFuncWrapper<NodeParsingTag, BooleanLiteralExpr, BoolConstant>(
			literal => new BoolConstant(literal.getValue()));
	}
}
