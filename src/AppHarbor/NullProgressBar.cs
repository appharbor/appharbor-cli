
namespace AppHarbor
{
	/// <summary>
	/// Reperesent a null progress bar which does nothing.
	/// </summary>
	public class NullProgressBar : IProgressBar
	{
		public void Update(string message, long processedItems, long totalItems)
		{
		}
	}
}