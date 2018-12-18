using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.github.javaparser;
using com.github.javaparser.ast;
using com.github.javaparser.ast.body;
using com.github.javaparser.ast.expr;
using com.github.javaparser.ast.stmt;
using com.github.javaparser.ast.type;
using com.github.javaparser.resolution.types;
using com.github.javaparser.symbolsolver;
using com.github.javaparser.symbolsolver.resolution.typesolvers;
using com.sun.org.apache.bcel.@internal.generic;
using ikvm.extensions;
using Infrastructure;
using java.io;
using java.nio.charset;
using java.util;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using SyntaxTree.Nodes;
using SyntaxTree.Types;
using TranspilerCore;
using TranspilerInfrastructure;
using Console = System.Console;
using Type = com.github.javaparser.ast.type.Type;

namespace JavaPlugin
{
	public class JavaProgramParser : IProgramParser
	{
		public Program ParseFromString(string sourceCode)
		{
			var symbolSolver = new JavaSymbolSolver(new ReflectionTypeSolver());
			CompilationUnit x = JavaParser.parse(new ByteArrayInputStream(sourceCode.getBytes(StandardCharsets.UTF_8)));
			symbolSolver.inject(x);
			if (x.getTypes().size() != 1) throw new ArgumentException("Exactly one top-level type is expected");

			Node rootClassNode = x.getTypes().get(0);
			if (!(rootClassNode is ClassOrInterfaceDeclaration rootClass && !rootClass.isInterface() && !rootClass.isAbstract() && rootClass.isPublic()))
			{
				throw new ArgumentException("Top-level type should be a public non-abstract class");
			}

			KernelBase kernel = new StandardKernel();
			kernel.Bind<IVariablesStorage>().To<VariablesStorage>().InSingletonScope();
			PartialFunctionCombiningMissingBindingResolver<TypeParsingTag>.LoadAllTaggedFunctions(kernel);
			PartialFunctionCombiningMissingBindingResolver<TypeParsingTag>.AddToKernel(kernel);
			PartialFunctionCombiningMissingBindingResolver<NodeParsingTag>.LoadAllTaggedFunctions(kernel);
			PartialFunctionCombiningMissingBindingResolver<NodeParsingTag>.AddToKernel(kernel);

			var parseStatement = kernel.Get<TaggedFunction<PartialFunctionCombined<NodeParsingTag>, Statement, IStatement>>();

			IStatement mainStatement = null;
			foreach (MethodDeclaration method in rootClass.getMethods().toArray())
			{
				if (!IsMethodMain(method)) throw new ArgumentException("Expected only main() method");
				mainStatement = parseStatement.Apply((Statement)method.getBody().get());
				if (mainStatement == null) throw new ArgumentException($"Unable to parse main() method statement: {method.getBody().get()}");
			}
			if (mainStatement == null) throw new ArgumentException("main() not found");
			return new Program(mainStatement);
		}

		private bool IsMethodMain(MethodDeclaration method)
		{
			if (method.getNameAsString() != "main") return false;
			if (method.getParameters().size() != 1) return false;
			Parameter param = method.getParameter(0);
			if (param.isVarArgs()) return false;
			if (param.getTypeAsString() != "String[]") return false;
			return true;
		}
	}
}