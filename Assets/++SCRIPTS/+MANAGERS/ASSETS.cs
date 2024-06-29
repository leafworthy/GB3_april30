using UnityEngine;
[ExecuteInEditMode]
public class ASSETS : Singleton<ASSETS>
{
	private void Start()
	{
		_fx = Resources.Load<FXAssets>("Assets/FX");
		if (_fx == null) return;
			_audio = Resources.Load<AudioAssets>("Assets/Audio");
		_levels = Resources.Load<LevelAssets>("Assets/Levels");
		_players = Resources.Load<CharacterPrefabAssets>("Assets/Players");
		_ui = Resources.Load<UIAssets>("Assets/UI");
	}


	public static FXAssets FX => I._fx ? I._fx : Resources.Load<FXAssets>("Assets/FX");
	private FXAssets _fx;

	public static AudioAssets sounds => I._audio ? I._audio : Resources.Load<AudioAssets>("Assets/Audio");
	private AudioAssets _audio;

	public static LevelAssets LevelAssets => I._levels ? I._levels : Resources.Load<LevelAssets>("Assets/Levels");
	private LevelAssets _levels;

	public static CharacterPrefabAssets Players => I._players ? I._players : Resources.Load<CharacterPrefabAssets>("Assets/Players");
	private CharacterPrefabAssets _players;

	public static UIAssets ui => I._ui? I._ui:  Resources.Load<UIAssets>("Assets/UI");
	private UIAssets _ui;
}
