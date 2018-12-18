using System;

namespace TranspilerInfrastructure
{
	public interface TaggedFunction<Tag, in T1, out TResult>
	{
		Func<T1, TResult> Apply { get; }
	}


	public class TaggedFuncWrapper<Tag, T1, TResult> : TaggedFunction<Tag, T1, TResult>
	{
		public Func<T1, TResult> Apply { get; }

		public TaggedFuncWrapper(Func<T1, TResult> func)
		{
			Apply = func;
		}
	}

	public static class TypeExtensions
	{
		public static bool IsTaggedFunction<Tag>(this Type type, out Type input, out Type output)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(TaggedFunction<,,>) && type.GetGenericArguments()[0] == typeof(Tag))
			{
				input = type.GetGenericArguments()[1];
				output = type.GetGenericArguments()[2];
				return true;
			}

			input = null;
			output = null;
			return false;
		}

		public static bool IsTaggedFunction<Tag>(this Type type) => type.IsTaggedFunction<Tag>(out _, out _);
	}
}
