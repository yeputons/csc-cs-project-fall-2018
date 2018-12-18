using System;
using CppPlugin;
using Infrastructure;
using SyntaxTree.Nodes;
using TranspilerInfrastructure;

namespace BinaryExpressionPlugin
{
	public class CppPrinter : TaggedFunction<ExpressionPrintingTag, BinaryExpression, string>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<ExpressionPrintingTag>, IExpression, string>>
			expressionPrinter;

		public CppPrinter(Lazy<TaggedFunction<PartialFunctionCombined<ExpressionPrintingTag>, IExpression, string>> expressionPrinter)
		{
			this.expressionPrinter = expressionPrinter;
		}

		public Func<BinaryExpression, string> Apply => (expr) =>
		{
			var left = expressionPrinter.Value.Apply(expr.Left) ??
			           throw new ArgumentException($"Unable to print {expr.Left}");
			var op = PrintOperation(expr.Operation);
			var right = expressionPrinter.Value.Apply(expr.Right) ??
			            throw new ArgumentException($"Unable to print {expr.Right}");
			// All operations are left-associative.
			if (GetPrecedence(expr.Left) > GetPrecedence(expr.Operation)) left = "(" + left + ")";
			if (GetPrecedence(expr.Right) >= GetPrecedence(expr.Operation)) right = "(" + right + ")";
			return left + " " + op + " " + right;
		};

		private static string PrintOperation(BinaryExpression.OperationType operation)
		{
			switch (operation)
			{
				case BinaryExpression.OperationType.Plus: return "+";
				case BinaryExpression.OperationType.Minus: return "-";
				case BinaryExpression.OperationType.Multiply: return "*";
				case BinaryExpression.OperationType.Divide: return "/";
				case BinaryExpression.OperationType.Eq: return "==";
				default: throw new ArgumentException($"Unknown operation: {operation}");
			}
		}

		private int GetPrecedence(IExpression expr)
		{
			if (expr is BinaryExpression binary)
			{
				return GetPrecedence(binary.Operation);
			}

			return 0;
		}

		private int GetPrecedence(BinaryExpression.OperationType operation)
		{
			switch (operation)
			{
				case BinaryExpression.OperationType.Multiply:
				case BinaryExpression.OperationType.Divide: return 1;
				case BinaryExpression.OperationType.Plus:
				case BinaryExpression.OperationType.Minus: return 2;
				case BinaryExpression.OperationType.Eq: return 3;
				default: throw new ArgumentException($"Unknown operation: {operation}");
			}
		}
	}
}
