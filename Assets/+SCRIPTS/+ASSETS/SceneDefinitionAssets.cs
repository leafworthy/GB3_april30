using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	/// <summary>
	/// Asset container for easily accessing scene definitions.
	/// </summary>
	[Serializable, CreateAssetMenu(fileName = "SceneDefinitions", menuName = "Gangsta Bean/Assets/Scene Definitions", order = 1)]
	public class SceneDefinitionAssets : ScriptableObject
	{
		public SceneDefinition startingScene;
		// Common scenes with direct references
		[Header("System Scenes")] public SceneDefinition gameManager;

		[Header("Menu Scenes")] public SceneDefinition mainMenu;
		public SceneDefinition characterSelect;
		public SceneDefinition restartLevel;
		public SceneDefinition endScreen;

		[Header("Gameplay Scenes")] public SceneDefinition gangstaBeanHouse;
		public SceneDefinition fancyHouse;
		public SceneDefinition baseballField;
		public SceneDefinition gasStation;

		[Header("Additional Scenes"), Tooltip("Additional scenes not covered by the properties above")]
		public List<SceneDefinition> additionalScenes = new();

		// Quick lookups
		private Dictionary<string, SceneDefinition> _nameMap;
		public SceneDefinition GameOverScene;
		public SceneDefinition WinScene;
		public SceneDefinition testScene;

		/// <summary>
		/// Initialize lookup dictionaries with all scene references
		/// </summary>
		public void Initialize()
		{
			_nameMap = new Dictionary<string, SceneDefinition>(StringComparer.OrdinalIgnoreCase);

			// Add all directly referenced scenes to name dictionary
			AddToNameDictionary(mainMenu);
			AddToNameDictionary(characterSelect);
			AddToNameDictionary(gangstaBeanHouse);
			AddToNameDictionary(endScreen);
			AddToNameDictionary(fancyHouse);
			AddToNameDictionary(baseballField);
			AddToNameDictionary(gasStation);
			AddToNameDictionary(restartLevel);
			AddToNameDictionary(gameManager);
			AddToNameDictionary(startingScene);
			AddToNameDictionary(GameOverScene);
			AddToNameDictionary(testScene);


			// Add all additional scenes
			if (additionalScenes != null)
			{
				foreach (var scene in additionalScenes)
				{
					AddToNameDictionary(scene);
				}
			}
		}

		// Add a scene to the name dictionary
		private void AddToNameDictionary(SceneDefinition scene)
		{
			_nameMap.TryAdd(scene.SceneName, scene);
		}



		/// <summary>
		/// Get all scenes as a flat list
		/// </summary>
		public List<SceneDefinition> GetAllScenes()
		{
			var result = new List<SceneDefinition>();
			foreach (var pair in _nameMap)
			{
				result.Add(pair.Value);
			}

			return result;
		}

		public void AddScene(SceneDefinition scene)
		{
			if (scene == null || !scene.IsValid())
				return;

			// Make sure name dictionary is initialized
			if (_nameMap == null)
				Initialize();

			// Add to name dictionary
			AddToNameDictionary(scene);


		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			// Automatically initialize when modified in editor
			Initialize();
		}
#endif
	}
}
