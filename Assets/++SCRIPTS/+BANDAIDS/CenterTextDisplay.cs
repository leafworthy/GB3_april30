namespace __SCRIPTS._BANDAIDS
{
	public static class CenterTextDisplay
	{
		private static ZombieWaveDisplay waveDisplay;
	
		public static void DisplayText(string textToDisplay)
		{
			if (waveDisplay == null)
			{
				waveDisplay = ZombieWaves.I.GetComponentInChildren<ZombieWaveDisplay>(true);
			}

			if (waveDisplay == null) return;
			waveDisplay.DisplayText(textToDisplay);
		}
	}
}