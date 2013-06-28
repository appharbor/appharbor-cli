
namespace AppHarbor
{
	/// <summary>
	/// Reperesent a null progress bar that does nothing.
	/// </summary>
	public class NullProgressBar : ProgressBarPresenter
	{
		public void Update(string message, long processedItems, long totalItems)
		{
		}
	}
}
