using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.github.javaparser.ast.expr;
using com.github.javaparser.ast.stmt;
using com.github.javaparser.resolution.types;
using com.github.javaparser.symbolsolver.javaparsermodel.declarations;
using Infrastructure;
using SyntaxTree.Nodes;
using SyntaxTree.Types;
using TranspilerInfrastructure;
using JavaAstVariableDeclarator = com.github.javaparser.ast.body.VariableDeclarator;

namespace JavaPlugin
{
	public interface IVariablesStorage
	{
		void Add(JavaAstVariableDeclarator key, IVariableDeclaration value);
		bool TryGetValue(JavaAstVariableDeclarator key, out IVariableDeclaration value);
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

		public bool TryGetValue(JavaAstVariableDeclarator key, out IVariableDeclaration value)
		{
			return variables.TryGetValue(key, out value);
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

	public class VariableReferenceParser : TaggedFunction<NodeParsingTag, NameExpr, IExpression>
	{
		private readonly IVariablesStorage storage;

		public VariableReferenceParser(IVariablesStorage storage)
		{
			this.storage = storage;
		}

		public Func<NameExpr, IExpression> Apply => (nameExpr) =>
		{
			var resolved = nameExpr.resolve();
			if (!(resolved is JavaParserSymbolDeclaration symbolDeclaration)) return null;
			IVariableDeclaration variable;
			if (!storage.TryGetValue((JavaAstVariableDeclarator) symbolDeclaration.getWrappedNode(), out variable)) return null;
			if (!(variable is VariableDeclaration variableDeclaration)) return null;
			return new VariableReference(variableDeclaration);
		};
	}
}
