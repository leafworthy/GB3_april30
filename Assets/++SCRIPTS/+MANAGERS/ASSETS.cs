using UnityEngine;

public class ASSETS : Singleton<ASSETS>
{
	private void Start()
	{
		_fx = Resources.Load<FXAssets>("Assets/FX");
		if (_fx == null) Debug.Break();
		_audio = Resources.Load<AudioAssets>("Assets/Audio");
		_levels = Resources.Load<LevelAssets>("Assets/Levels");
		_players = Resources.Load<CharacterPrefabAssets>("Assets/Players");
		_ui = Resources.Load<UIAssets>("Assets/UI");
	}


	public static FXAssets FX => I._fx;
	private FXAssets _fx;

	public static AudioAssets sounds => I._audio;
	private AudioAssets _audio;

	public static LevelAssets LevelAssets => I._levels;
	private LevelAssets _levels;

	public static CharacterPrefabAssets Players => I._players;
	private CharacterPrefabAssets _players;

	public static UIAssets ui => I._ui;
	private UIAssets _ui;
}
