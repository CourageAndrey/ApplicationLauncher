namespace ApplicationLauncher
{
	/// <summary>
	/// Update checking method.
	/// </summary>
	public enum CheckUpdateStrategy
	{
		/// <summary>
		/// Do not check at all.
		/// </summary>
		Never,

		/// <summary>
		/// Check update, but ask user if install is needed or not.
		/// </summary>
		AskBeforeInstall,

		/// <summary>
		/// Check and install automatically.
		/// </summary>
		Automatically,
	}
}
