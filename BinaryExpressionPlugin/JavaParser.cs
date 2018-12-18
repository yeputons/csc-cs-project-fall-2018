using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if (op == BinaryExpr.Operator.PLUS) return BinaryExpression.OperationType.PLUS;
			if (op == BinaryExpr.Operator.MINUS) return BinaryExpression.OperationType.MINUS;
			if (op == BinaryExpr.Operator.MULTIPLY) return BinaryExpression.OperationType.MULTIPLY;
			if (op == BinaryExpr.Operator.DIVIDE) return BinaryExpression.OperationType.DIVIDE;
			if (op == BinaryExpr.Operator.EQUALS) return BinaryExpression.OperationType.EQ;
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
