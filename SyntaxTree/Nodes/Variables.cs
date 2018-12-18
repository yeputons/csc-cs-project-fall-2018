using System;
using System.Collections.Generic;
using System.Text;
using SyntaxTree.Types;

namespace SyntaxTree.Nodes
{
	public interface IVariableDeclaration
	{
		string Name { get; }
		IType Type { get; }
	}

	public class VariableDeclaration : IStatement, IVariableDeclaration
	{
		public VariableDeclaration(string name, IType type, IExpression initiailizer)
		{
			if (name == null) throw new ArgumentException("VariableDeclaration.Name should be non-null");
			if (type == null) throw new ArgumentException("VariableDeclaration.Type should be non-null");
			if (initiailizer != null && type != initiailizer.EvaluationType) throw new ArgumentException("VariableDeclaration.Initializer should have the same type as variable");
			Name = name;
			Type = type;
			Initiailizer = initiailizer;
			Children = new List<INode>();
		}

		public string Name { get; }
		public IType Type { get; }
		public IExpression Initiailizer { get; }
		public IReadOnlyList<INode> Children { get; }
	}

}
