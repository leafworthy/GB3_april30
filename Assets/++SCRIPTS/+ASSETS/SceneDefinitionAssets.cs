using System;
using System.Collections.Generic;
using UnityEngine;

namespace GangstaBean.Assets
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
			if (scene == null || !scene.IsValid())
				return;

			// Add to name map, using scene name as key
			if (!string.IsNullOrEmpty(scene.SceneName) && !_nameMap.ContainsKey(scene.SceneName)) _nameMap[scene.SceneName] = scene;
		}

	
		public SceneDefinition GetByName(string sceneName)
		{
			if (string.IsNullOrEmpty(sceneName))
			{
				Debug.Log("Scene name is null or empty");
				return null;
			}

			if (_nameMap == null)
				Initialize();

			var scene = _nameMap[sceneName];
			if (scene == null) Debug.Log("can't find scene");

			Debug.Log("got scene by name: " + sceneName);
			return scene;
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

		public SceneDefinition FindScene(string sceneName) => GetByName(sceneName);

		public void AddScene(SceneDefinition scene)
		{
			if (scene == null || !scene.IsValid())
				return;

			// Make sure name dictionary is initialized
			if (_nameMap == null)
				Initialize();

			// Add to name dictionary
			AddToNameDictionary(scene);

			// Add to additional scenes list if not present in any other property
			if (scene != mainMenu && scene != characterSelect && scene != gangstaBeanHouse && scene != endScreen && scene != fancyHouse &&
			    scene != baseballField && scene != gasStation && scene != restartLevel && scene != gameManager && !additionalScenes.Contains(scene))
				additionalScenes.Add(scene);
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