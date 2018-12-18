namespace SyntaxTree.Types
{
	public interface SCollection : IType
    {
		IType Underlying { get; }
    }

	public class SList : SCollection
	{
		public SList(IType underlying)
		{
			this.Underlying = underlying;
		}

		public IType Underlying { get; }
	}
}