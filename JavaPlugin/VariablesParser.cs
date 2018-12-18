using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.github.javaparser.ast.expr;
using com.github.javaparser.ast.stmt;
using com.github.javaparser.resolution.types;
using Infrastructure;
using SyntaxTree.Nodes;
using SyntaxTree.Types;
using TranspilerInfrastructure;
using JavaAstVariableDeclarator = com.github.javaparser.ast.body.VariableDeclarator;

namespace JavaPlugin
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

	public interface IVariablesStorage
	{
		void Add(JavaAstVariableDeclarator key, IVariableDeclaration value);
		IVariableDeclaration this[JavaAstVariableDeclarator key] { get; }
		ICollection<JavaAstVariableDeclarator> Keys { get; }
		ICollection<IVariableDeclaration> Values { get; }
	}

	public class VariablesStorage : IVariablesStorage
	{
		private readonly Dictionary<JavaAstVariableDeclarator, IVariableDeclaration> variables = new Dictionary<JavaAstVariableDeclarator, IVariableDeclaration>();

		public void Add(JavaAstVariableDeclarator key, IVariableDeclaration value)
		{
			variables.Add(key, value);;
		}

		public IVariableDeclaration this[JavaAstVariableDeclarator key] => variables[key];

		public ICollection<JavaAstVariableDeclarator> Keys { get => variables.Keys; }
		public ICollection<IVariableDeclaration> Values { get => variables.Values; }
	}

	public class VariablesDeclarationParser : TaggedFunction<NodeParsingTag, ExpressionStmt, IStatement>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, JavaAstVariableDeclarator, IStatement>> variableDeclarationParser;

		public VariablesDeclarationParser(Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, JavaAstVariableDeclarator, IStatement>> variableDeclarationParser)
		{
			this.variableDeclarationParser = variableDeclarationParser;
		}

		public Func<ExpressionStmt, IStatement> Apply => (exprStmt) =>
		{
			if (!(exprStmt.getExpression() is VariableDeclarationExpr expr)) return null;
			var variables = expr.getVariables().toArray().Select(variable =>
			{
				var parsed = variableDeclarationParser.Value.Apply((JavaAstVariableDeclarator) variable);
				if (parsed == null) throw new ArgumentException($"Unable to parse variable declaration: {variable}");
				return parsed;
			}).ToList();
			if (variables.Count == 1)
				return variables[0];
			return new BlockStatement(variables);
		};
	}

	public class VariableDeclaratorParser : TaggedFunction<NodeParsingTag, JavaAstVariableDeclarator, IStatement>
	{
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<TypeParsingTag>, ResolvedType, IType>> typeParser;
		private readonly Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>> expressionParser;
		private readonly IVariablesStorage storage;

		public VariableDeclaratorParser(Lazy<TaggedFunction<PartialFunctionCombined<TypeParsingTag>, ResolvedType, IType>> typeParser, Lazy<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Expression, IExpression>> expressionParser, IVariablesStorage storage)
		{
			this.typeParser = typeParser;
			this.expressionParser = expressionParser;
			this.storage = storage;
		}

		public Func<JavaAstVariableDeclarator, IStatement> Apply => (javaAst) =>
		{
			var javaResolvedType = javaAst.getType().resolve();
			var type = typeParser.Value.Apply(javaResolvedType) ?? throw new ArgumentException($"Unable to parse type: {javaResolvedType}");
			var javaInitializer = (Expression) javaAst.getInitializer().orElse(null);
			IExpression initializer = null;
			if (javaInitializer != null)
			{
				initializer = expressionParser.Value.Apply(javaInitializer) ??
				              throw new ArgumentException($"Unable to parse variable initialization: {javaInitializer}");
			}
			var result = new VariableDeclaration(javaAst.getNameAsString(), type, initializer);
			storage.Add(javaAst, result);
			return result;
		};
	}
}
