using System.Collections;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteInEditMode]
	public class AssetManager : MonoBehaviour, IService
	{
		// Level Assets
		private LevelAssets _levels;
		public LevelAssets LevelAssets => _levels ?? Resources.Load<LevelAssets>("Assets/Levels");

		private GlobalVars _vars;
		public GlobalVars Vars => _vars ?? Resources.Load<GlobalVars>("Assets/GlobalVars");

		// Character/player Assets
		private CharacterPrefabAssets _players;
		public CharacterPrefabAssets Players => _players ?? Resources.Load<CharacterPrefabAssets>("Assets/Players");

		// UI Assets
		private UIAssets _ui;
		public UIAssets UI => _ui ?? Resources.Load<UIAssets>("Assets/UI");

		// FX Assets
		private FXAssets _fx;
		public FXAssets FX => _fx = Resources.Load<FXAssets>("Assets/FX");

		private HouseAssets _house;

		private SceneDefinitionAssets _scenes;
		public SceneDefinitionAssets Scenes => _scenes ?? Resources.Load<SceneDefinitionAssets>("Assets/SceneDefinitions");

		public void StartService()
		{
			if (Application.isEditor && !Application.isPlaying) LoadAssets();
		}

		private void LoadAssets()
		{
			if (_levels == null || _players == null || _ui == null || _fx == null || _scenes == null) StartCoroutine(LoadAssetsAsync());
		}

		private IEnumerator LoadAssetsAsync()
		{
			if (_levels == null)
				_levels = Resources.Load<LevelAssets>("Assets/Levels");

			yield return null;

			if (_players == null)
				_players = Resources.Load<CharacterPrefabAssets>("Assets/Players");

			yield return null;

			if (_ui == null)
				_ui = Resources.Load<UIAssets>("Assets/UI");

			yield return null;

			if (_fx == null)
				_fx = Resources.Load<FXAssets>("Assets/FX");

			yield return null;

			if (_scenes == null)
				_scenes = Resources.Load<SceneDefinitionAssets>("Assets/SceneDefinitions");

			// Initialize the scene definitions
			if (_scenes != null)
				_scenes.Initialize();
		}
	}
}
