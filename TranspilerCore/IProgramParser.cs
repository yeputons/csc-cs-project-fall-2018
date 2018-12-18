using SyntaxTree.Nodes;

namespace TranspilerCore
{
	public interface IProgramParser
	{
		Program ParseFromString(string sourceCode);
	}
}