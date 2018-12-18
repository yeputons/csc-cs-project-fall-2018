using System;
using Infrastructure;
using SyntaxTree.Nodes;
using SyntaxTree.Types;
using TranspilerInfrastructure;

namespace CppPlugin
{
	interface StatementPrintingTag
	{
	}

	class BlockStatementPrinter : TaggedFunction<StatementPrintingTag, BlockStatement, bool>
	{
		private readonly IndentedTextWriter writer;

		private readonly Lazy<TaggedFunction<PartialFunctionCombined<StatementPrintingTag>, IStatement, bool>>
			statementPrinter;

		public BlockStatementPrinter(IndentedTextWriter writer, Lazy<TaggedFunction<PartialFunctionCombined<StatementPrintingTag>, IStatement, bool>> statementPrinter)
		{
			this.writer = writer;
			this.statementPrinter = statementPrinter;
		}

		public Func<BlockStatement, bool> Apply => (stmt) =>
		{
			writer.WriteLine("{");
			writer.WithIndent(delegate
			{
				foreach (var child in stmt.Statements)
				{
					if (!statementPrinter.Value.Apply(child)) throw new ArgumentException($"Unable to print statement {child}");
				}
			});
			writer.WriteLine("}");
			return true;
		};
	}

	class VariableDeclarationPrinter : TaggedFunction<StatementPrintingTag, VariableDeclaration, bool>
	{
		private readonly IndentedTextWriter writer;

		private readonly Lazy<TaggedFunction<PartialFunctionCombined<TypePrintingTag>, IType, string>>
			typePrinter;

		private readonly Lazy<TaggedFunction<PartialFunctionCombined<ExpressionPrintingTag>, IExpression, string>>
			expressionPrinter;

		public VariableDeclarationPrinter(IndentedTextWriter writer, Lazy<TaggedFunction<PartialFunctionCombined<TypePrintingTag>, IType, string>> typePrinter, Lazy<TaggedFunction<PartialFunctionCombined<ExpressionPrintingTag>, IExpression, string>> expressionPrinter)
		{
			this.writer = writer;
			this.typePrinter = typePrinter;
			this.expressionPrinter = expressionPrinter;
		}

		public Func<VariableDeclaration, bool> Apply => (stmt) =>
		{
			var type = typePrinter.Value.Apply(stmt.Type) ?? throw new ArgumentException($"Unable to show type: {stmt.Type}");
			var name = stmt.Name;
			if (stmt.Initiailizer == null)
			{
				writer.WriteLine(type + " " + name + ";");
			}
			else
			{
				var expr = expressionPrinter.Value.Apply(stmt.Initiailizer) ?? throw new ArgumentException($"Unable to show initializer: {stmt.Initiailizer}");
				writer.WriteLine(type + " " + name + " = " + expr + ";");
			}
			return true;
		};
	}
}
