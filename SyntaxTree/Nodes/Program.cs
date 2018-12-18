using System;

namespace SyntaxTree.Nodes
{
	public class Program
	{
		public Program(IStatement mainStatement)
		{
			MainStatement = mainStatement ?? throw new ArgumentException("Program expects a non-null mainStatement");
		}

		public IStatement MainStatement { get; }
	}
}
