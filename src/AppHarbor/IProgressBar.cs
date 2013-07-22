namespace AppHarbor
{
	/// <summary>
	/// Represent a progress bar that show status updates.
	/// </summary>
	public interface IProgressBar
	{
		/// <summary>
		/// Show update on the progress.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="processedItems"></param>
		/// <param name="totalItems"></param>
		void Update(string message, long processedItems, long totalItems);
	}
}
