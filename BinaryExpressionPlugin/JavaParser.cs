using System;
using com.github.javaparser.ast.expr;
using Infrastructure;
using JavaPlugin;
using SyntaxTree.Nodes;
using TranspilerInfrastructure;

namespace BinaryExpressionPlugin
{
	class BinaryExprJavaParser : TaggedFunction<NodeParsingTag, BinaryExpr, BinaryExpression>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>>
			expressionParser;

		public BinaryExprJavaParser(Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>> expressionParser)
		{
			this.expressionParser = expressionParser;
		}

		public Func<BinaryExpr, BinaryExpression> Apply => (expr) =>
		{
			var operation = getOperation(expr.getOperator());
			var left = expressionParser.Value.Apply(expr.getLeft()) ?? throw new ArgumentException($"Unable to parse {expr.getLeft()}");
			var right = expressionParser.Value.Apply(expr.getRight()) ?? throw new ArgumentException($"Unable to parse {expr.getRight()}");
			return new BinaryExpression(left, operation, right);
		};

		private static BinaryExpression.OperationType getOperation(BinaryExpr.Operator op)
		{
			if (op == BinaryExpr.Operator.PLUS) return BinaryExpression.OperationType.Plus;
			if (op == BinaryExpr.Operator.MINUS) return BinaryExpression.OperationType.Minus;
			if (op == BinaryExpr.Operator.MULTIPLY) return BinaryExpression.OperationType.Multiply;
			if (op == BinaryExpr.Operator.DIVIDE) return BinaryExpression.OperationType.Divide;
			if (op == BinaryExpr.Operator.EQUALS) return BinaryExpression.OperationType.Eq;
			throw new ArgumentException($"Unsupported binary operation: {op}");
		}
	}

	class EnclosedExprJavaParser : TaggedFunction<NodeParsingTag, EnclosedExpr, IExpression>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>>
			expressionParser;

		public EnclosedExprJavaParser(Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>> expressionParser)
		{
			this.expressionParser = expressionParser;
		}

		public Func<EnclosedExpr, IExpression> Apply => (expr) => expressionParser.Value.Apply(expr.getInner());
	}
}
