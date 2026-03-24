using System.Collections;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteInEditMode]
	public class AssetManager : MonoBehaviour, IService
	{
		public LevelAssets LevelAssets => _levels ??= Resources.Load<LevelAssets>("Assets/Levels");
		LevelAssets _levels;

		public GlobalVars Vars => _vars ??= Resources.Load<GlobalVars>("Assets/GlobalVars");
		GlobalVars _vars;

		public CharacterPrefabAssets Players => _players ??= Resources.Load<CharacterPrefabAssets>("Assets/Players");
		CharacterPrefabAssets _players;

		public UIAssets UI => _ui ??= Resources.Load<UIAssets>("Assets/UI");
		UIAssets _ui;

		public FXAssets FX => _fx ??= Resources.Load<FXAssets>("Assets/FX");
		FXAssets _fx;

		public SceneDefinitionAssets Scenes => _scenes ??= Resources.Load<SceneDefinitionAssets>("Assets/SceneDefinitions");
		SceneDefinitionAssets _scenes;

		public void StartService()
		{
			if (Application.isEditor && !Application.isPlaying) LoadAssets();
		}

		void LoadAssets()
		{
			StartCoroutine(LoadAssetsAsync());
		}

		IEnumerator LoadAssetsAsync()
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
