using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// ScriptableObject to define a scene with its metadata
    /// and provide loading functionality
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "New Scene Definition", menuName = "Gangsta Bean/Scene Definition", order = 2)]
    public class SceneDefinition : ScriptableObject
    {
        // In the Unity Editor, we reference the actual scene asset
        [Header("Scene Reference"), Tooltip("The actual Unity scene asset")]
#if UNITY_EDITOR
        public UnityEditor.SceneAsset sceneAsset;
#endif

        [Header("Scene Info")]
        public string sceneName;

        public string displayName;

        [Tooltip("Description of the scene"), TextArea(3, 5)]
        public string description;

        [Tooltip("Image shown during level transition")]
        public Sprite sceneImage;

        [Header("Loading Screen")]
        public bool requiresButtonPressToLoad = false;


        [Header("Audio"), Tooltip("Music track to play in this scene")]
        public AudioClip backgroundMusic;

        // Scene path is derived from the asset (valid in editor only)
        public string scenePath;
        private List<TravelPoint> spawnPoints;

        /// <summary>
        /// Get the scene name
        /// </summary>
        public string SceneName => sceneName;

        /// <summary>
        /// Gets the display name, or the scene name if no display name is set
        /// </summary>
        public string DisplayName => !string.IsNullOrEmpty(displayName) ? displayName : FormatForDisplay(name);

        /// <summary>
        /// The path to the scene asset
        /// </summary>
        public string ScenePath => scenePath;

        /// <summary>
        /// Checks if this scene definition is valid
        /// </summary>
        public bool IsValid() => !string.IsNullOrEmpty(sceneName);

        private AssetManager assetManager;
        private AssetManager AssetManager => assetManager ?? ServiceLocator.Get<AssetManager>();




        // Implicit conversion to string
        public static implicit operator string(SceneDefinition definition) =>
            definition?.SceneName ?? string.Empty;

        // Equality methods
        public bool Equals(SceneDefinition other) =>
            other != null && string.Equals(SceneName, other.SceneName, StringComparison.OrdinalIgnoreCase);

        public bool Equals(string _sceneName) =>
            !string.IsNullOrEmpty(_sceneName) && string.Equals(SceneName, _sceneName, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj)
        {
            if (obj is SceneDefinition other)
                return Equals(other);

            if (obj is string sceneName)
                return Equals(sceneName);

            return false;
        }

        public override int GetHashCode() => SceneName?.GetHashCode() ?? 0;

        public override string ToString() => $"{DisplayName} ({SceneName})";

        /// <summary>
        /// Create a default SceneDefinition from a scene name
        /// </summary>
        public static SceneDefinition CreateDefault(string sceneName)
        {
            var definition = CreateInstance<SceneDefinition>();
            definition.sceneName = sceneName;
            definition.displayName = FormatForDisplay(sceneName);
            definition.description = $"Scene: {sceneName}";


            return definition;
        }



        // Format a string from camelCase to spaced words
        public static string FormatForDisplay(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Remove "Scene" suffix if present
            if (name.EndsWith("Scene"))
                name = name.Substring(0, name.Length - 5);

            var result = "";
            for (var i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]))
                    result += " ";
                result += name[i];
            }

            return result;
        }

#if UNITY_EDITOR
        // Validate the scene definition in the editor
        private void OnValidate()
        {
            // Update scene name and path from scene asset
            if (sceneAsset != null)
            {
                scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
                sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                // Set default display name if empty
                if (string.IsNullOrEmpty(displayName))
                    displayName = FormatForDisplay(sceneName);

            }
        }

        // Helper method to find a SceneAsset by name
        public static UnityEditor.SceneAsset FindSceneAsset(string sceneName)
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene " + sceneName);
            if (guids.Length > 0)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(assetPath);
            }

            return null;
        }
#endif

    }
}
