namespace ApplicationLauncher
{
	/// <summary>
	/// Type of update.
	/// </summary>
	public enum UpdateType
	{
		/// <summary>
		/// Standard one, can be easily skipped.
		/// </summary>
		Regular,

		/// <summary>
		/// Recommended to install, but also can be skipped.
		/// </summary>
		HighlyRecommended,

		/// <summary>
		/// Required - can not be skipped.
		/// </summary>
		Required,
	}
}
