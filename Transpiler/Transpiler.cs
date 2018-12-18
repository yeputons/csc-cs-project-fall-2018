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
	        Program p = new JavaProgramParser().ParseFromString(File.ReadAllText("TestInput.java"));
	        System.Console.Write(new CppPrinter().PrintToString(p));
        }
    }
}