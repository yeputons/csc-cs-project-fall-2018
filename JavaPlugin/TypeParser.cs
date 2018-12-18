using com.github.javaparser.resolution.types;
using SyntaxTree.Types;
using TranspilerInfrastructure;

namespace JavaPlugin
{
	public interface TypeParsingTag
	{
	}

	public static class TypeParser
	{
		public static readonly TaggedFunction<TypeParsingTag, ResolvedPrimitiveType, SInt32> ParseInt32 = new TaggedFuncWrapper<TypeParsingTag, ResolvedPrimitiveType, SInt32>(
			type => (type == ResolvedPrimitiveType.INT) ? SInt32.Instance : null);
		public static readonly TaggedFunction<TypeParsingTag, ResolvedPrimitiveType, SInt64> ParseInt64 = new TaggedFuncWrapper<TypeParsingTag, ResolvedPrimitiveType, SInt64>(
			type => (type == ResolvedPrimitiveType.LONG) ? SInt64.Instance : null);
		public static readonly TaggedFunction<TypeParsingTag, ResolvedPrimitiveType, SChar> ParseChar = new TaggedFuncWrapper<TypeParsingTag, ResolvedPrimitiveType, SChar>(
			type => (type == ResolvedPrimitiveType.CHAR) ? SChar.Instance : null);
		public static readonly TaggedFunction<TypeParsingTag, ResolvedPrimitiveType, SBoolean> ParseBoolean = new TaggedFuncWrapper<TypeParsingTag, ResolvedPrimitiveType, SBoolean>(
			type => (type == ResolvedPrimitiveType.BOOLEAN) ? SBoolean.Instance : null);
	}
}
