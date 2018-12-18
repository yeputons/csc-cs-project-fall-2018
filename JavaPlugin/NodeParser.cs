using System;
using System.Linq;
using com.github.javaparser.ast.stmt;
using Infrastructure;
using SyntaxTree.Nodes;
using TranspilerInfrastructure;

namespace JavaPlugin
{
	public interface NodeParsingTag
	{
	}

	public class BlockStmtParser : TaggedFunction<NodeParsingTag, BlockStmt, IStatement>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Statement, IStatement>>
			statementParser;

		public BlockStmtParser(
			Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Statement, IStatement>> statementParser)
		{
			this.statementParser = statementParser;
		}

		public Func<BlockStmt, IStatement> Apply => block =>
			new BlockStatement(block.getStatements().toArray()
				.Select(s =>
					statementParser.Value.Apply((Statement)s) ?? throw new ArgumentException($"Unable to parse statement: {s}")
				).ToList());
	}
}