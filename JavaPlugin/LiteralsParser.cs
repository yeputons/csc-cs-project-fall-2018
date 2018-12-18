using com.github.javaparser.ast.expr;
using SyntaxTree.Nodes;
using TranspilerInfrastructure;

namespace JavaPlugin
{
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
