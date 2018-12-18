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
			this.data = statements;
		}

		private readonly IReadOnlyList<IStatement> data;

		public IReadOnlyList<INode> Children => data;
	}
}