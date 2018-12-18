using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if (GetPrecedence(expr.Left) > GetPrecedence(expr.Operation)) left = "(" + left + ")";
			if (GetPrecedence(expr.Right) > GetPrecedence(expr.Operation)) right = "(" + right + ")";
			return left + " " + op + " " + right;
		};

		private static string PrintOperation(BinaryExpression.OperationType operation)
		{
			switch (operation)
			{
				case BinaryExpression.OperationType.PLUS: return "+";
				case BinaryExpression.OperationType.MINUS: return "-";
				case BinaryExpression.OperationType.MULTIPLY: return "*";
				case BinaryExpression.OperationType.DIVIDE: return "/";
				case BinaryExpression.OperationType.EQ: return "==";
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
				case BinaryExpression.OperationType.MULTIPLY:
				case BinaryExpression.OperationType.DIVIDE: return 1;
				case BinaryExpression.OperationType.PLUS:
				case BinaryExpression.OperationType.MINUS: return 2;
				case BinaryExpression.OperationType.EQ: return 3;
				default: throw new ArgumentException($"Unknown operation: {operation}");
			}
		}
	}
}
