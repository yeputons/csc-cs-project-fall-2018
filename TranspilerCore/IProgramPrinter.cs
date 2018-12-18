using SyntaxTree.Nodes;

namespace TranspilerCore
{
    public interface IProgramPrinter
    {
	    string PrintToString(Program program);
    }
}
