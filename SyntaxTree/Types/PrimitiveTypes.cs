namespace SyntaxTree.Types
{
    public class SInt32 : IType
    {
		public static SInt32 Instance = new SInt32();

		private SInt32() { }
    }

	public class SInt64 : IType
	{
		public static SInt64 Instance = new SInt64();

		private SInt64() { }
	}

	public class SString : IType
    {
	    public static SString Instance = new SString();

	    private SString() { }
    }

	public class SChar : IType
    {
	    public static SChar Instance = new SChar();

	    private SChar() { }
    }

	public class SBoolean : IType
    {
	    public static SBoolean Instance = new SBoolean();

	    private SBoolean() { }
    }
}