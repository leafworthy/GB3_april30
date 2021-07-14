public class ASSETS : Singleton<ASSETS>
{
	public static FXAssets FX
	{
		get { return I._fx;}
	}

	public FXAssets _fx;
	public static AudioAssets sounds
	{
		get { return I._audio; }
	}


	public AudioAssets _audio;

	public static LevelAssets LevelAssets
	{
		get { return I.levels; }
	}

	public LevelAssets levels;

	public CharacterPrefabAssets characterPrefabs;

	public static CharacterPrefabAssets CharacterPrefabs
	{
		get { return I.characterPrefabs; }
	}
}
