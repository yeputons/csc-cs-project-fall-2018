using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxTree.Nodes
{
	public class BlockStatement : IStatement
	{
		public BlockStatement(IReadOnlyList<IStatement> statements)
		{
			if (statements == null || statements.Contains(null))
				throw new ArgumentException(
					$"BlockStatement expects non-null list of non-null statements, got {statements}");
			Statements = statements;
		}

		public IReadOnlyList<IStatement> Statements { get; }

		public IReadOnlyList<INode> Children => Statements;
	}
}