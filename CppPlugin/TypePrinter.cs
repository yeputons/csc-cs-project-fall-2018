using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxTree.Types;
using TranspilerInfrastructure;

namespace CppPlugin
{
	public interface TypePrintingTag
	{
	}

	public static class TypePrinter
	{
		public static readonly TaggedFunction<TypePrintingTag, SInt32, string> PrintInt32 = new TaggedFuncWrapper<TypePrintingTag, SInt32, string>(_ => "int");
		public static readonly TaggedFunction<TypePrintingTag, SInt64, string> PrintInt64 = new TaggedFuncWrapper<TypePrintingTag, SInt64, string>(_ => "long long");
		public static readonly TaggedFunction<TypePrintingTag, SChar, string> PrintChar = new TaggedFuncWrapper<TypePrintingTag, SChar, string>(_ => "char");
		public static readonly TaggedFunction<TypePrintingTag, SBoolean, string> PrintBool = new TaggedFuncWrapper<TypePrintingTag, SBoolean, string>(_ => "bool");
	}
}
