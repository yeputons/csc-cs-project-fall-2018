using System;
using System.IO;
using System.Text;
using Infrastructure;
using Ninject;
using SyntaxTree.Nodes;
using TranspilerCore;
using TranspilerInfrastructure;

namespace CppPlugin
{
    public class CppPrinter : IProgramPrinter
    {
	    public string PrintToString(Program program)
	    {
			var sb = new StringBuilder();

		    IKernel kernel = new StandardKernel();
		    PartialFunctionCombiningMissingBindingResolver<TypePrintingTag>.LoadAllTaggedFunctions(kernel);
		    PartialFunctionCombiningMissingBindingResolver<TypePrintingTag>.AddToKernel(kernel);
		    PartialFunctionCombiningMissingBindingResolver<ExpressionPrintingTag>.LoadAllTaggedFunctions(kernel);
		    PartialFunctionCombiningMissingBindingResolver<ExpressionPrintingTag>.AddToKernel(kernel);
			PartialFunctionCombiningMissingBindingResolver<StatementPrintingTag>.LoadAllTaggedFunctions(kernel);
		    PartialFunctionCombiningMissingBindingResolver<StatementPrintingTag>.AddToKernel(kernel);

			using (IndentedTextWriter writer = new IndentedTextWriter(new StringWriter(sb)))
			{
				kernel.Bind<IndentedTextWriter>().ToConstant(writer);
				writer.WriteLines(
@"#include <cstdio>
#include <cstdlib>
#include <algorithm>
#include <iostream>
#include <vector>
#include <string>

using std::vector;
using std::cin;
using std::cout;
using std::max;
using std::min;
using std::string;
using std::to_string;

int main()
{
    std::ios_base::sync_with_stdio(false);");
				writer.WithIndent(delegate {
					var printStatement = kernel.Get<TaggedFunction<PartialFunctionCombined<StatementPrintingTag>, IStatement, bool>>();
					if (!printStatement.Apply(program.MainStatement)) throw new ArgumentException("Unable to print main statement");
					writer.WriteLine("return 0;");
				});
			    writer.WriteLine("}");
		    }
			return sb.ToString();
	    }
    }
}
