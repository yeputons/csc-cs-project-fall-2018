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

	class ExpressionPrinter
	{
		public static readonly TaggedFunction<ExpressionPrintingTag, Int32Constant, string> PrintInt32Constant = new TaggedFuncWrapper<ExpressionPrintingTag, Int32Constant, string>(c => c.Value.ToString());
		public static readonly TaggedFunction<ExpressionPrintingTag, Int64Constant, string> PrintInt64Constant = new TaggedFuncWrapper<ExpressionPrintingTag, Int64Constant, string>(c => c.Value.ToString() + "LL");
		public static readonly TaggedFunction<ExpressionPrintingTag, CharConstant, string> PrintCharConstant = new TaggedFuncWrapper<ExpressionPrintingTag, CharConstant, string>(c =>
		{
			switch (c.Value)
			{
				case '\n': return "'\\n'";
				case '\'': return "'\\''";
				default: return "'" + c.Value + "'";
			}
		});

		public static readonly TaggedFunction<ExpressionPrintingTag, BoolConstant, string> PrintBoolConstant = new TaggedFuncWrapper<ExpressionPrintingTag, BoolConstant, string>(c => c.Value ? "true" : "false");

		public static readonly TaggedFunction<ExpressionPrintingTag, VariableReference, string> PrintVariableReference = new TaggedFuncWrapper<ExpressionPrintingTag, VariableReference, string>(r => r.Declaration.Name);
	}
}
