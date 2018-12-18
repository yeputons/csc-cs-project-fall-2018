using SyntaxTree.Nodes;

namespace SyntaxTree.Nodes
{
    public class Identifier
    {
	    public Identifier(INode declaration, string shortName)
	    {
		    Declaration = declaration;
		    ShortName = shortName;
	    }

	    public INode Declaration { get; }
        public string ShortName { get; }
    }
}