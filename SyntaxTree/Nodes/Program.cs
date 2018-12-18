using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SyntaxTree.Nodes
{
	public class Program
	{
		public Program(INode mainStatement)
		{
			MainStatement = mainStatement ?? throw new ArgumentException("Program expects a non-null mainStatement");
		}

		public INode MainStatement { get; }
	}
}
