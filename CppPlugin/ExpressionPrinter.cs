using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxTree.Nodes;
using TranspilerInfrastructure;

namespace CppPlugin
{
	interface ExpressionPrintingTag
	{
	}

	class LiteralsPrinter
	{
		public static readonly TaggedFunction<StatementPrintingTag, Int32Constant, string> PrintInt32Constant = new TaggedFuncWrapper<StatementPrintingTag, Int32Constant, string>(c => c.Value.ToString());
		public static readonly TaggedFunction<StatementPrintingTag, Int64Constant, string> PrintInt64Constant = new TaggedFuncWrapper<StatementPrintingTag, Int64Constant, string>(c => c.Value.ToString() + "LL");
		public static readonly TaggedFunction<StatementPrintingTag, CharConstant, string> PrintCharConstant = new TaggedFuncWrapper<StatementPrintingTag, CharConstant, string>(c =>
		{
			switch (c.Value)
			{
				case '\n': return "'\\n'";
				case '\'': return "'\\''";
				default: return "'" + c.Value + "'";
			}
		});

		public static readonly TaggedFunction<StatementPrintingTag, BoolConstant, string> PrintBoolConstant = new TaggedFuncWrapper<StatementPrintingTag, BoolConstant, string>(c => c.Value ? "true" : "false");
	}
}
