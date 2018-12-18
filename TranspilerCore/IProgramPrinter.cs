using System;
using System.Collections.Generic;
using System.Text;
using SyntaxTree.Nodes;

namespace TranspilerCore
{
    public interface IProgramPrinter
    {
	    string PrintToString(Program program);
    }
}
