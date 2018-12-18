using System;
using System.IO;
using CppPlugin;
using JavaPlugin;
using SyntaxTree.Nodes;

namespace Transpiler
{
	class Transpiler
    {
        static void Main(string[] args)
        {
	        Type t = typeof(BinaryExpressionPlugin.BinaryExpression);  // Load assembly.

	        Program p = new JavaProgramParser().ParseFromString(File.ReadAllText("TestInput.java"));
	        Console.Write(new CppPrinter().PrintToString(p));
        }
    }
}