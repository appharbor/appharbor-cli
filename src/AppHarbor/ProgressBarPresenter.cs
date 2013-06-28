namespace AppHarbor
{
    /// <summary>
    /// Represent a presenter for progress bar information.
    /// </summary>
    public interface ProgressBarPresenter
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
