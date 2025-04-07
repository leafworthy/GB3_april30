using System.Collections;
using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Unified Asset Management System.
    /// Consolidates ASSETS, FX, UI, and Scenes into a single system.
    /// 
    /// USAGE GUIDE:
    /// - ASSETS.LevelAssets - For level and layer mask references
    /// - ASSETS.Players - For character and enemy prefabs
    /// - ASSETS.ui - For UI prefabs and elements
    /// - ASSETS.FX - For effects, particles, and visual elements
    /// - ASSETS.Scenes - For scene definitions and references
    /// </summary>
    [ExecuteInEditMode]
    public class ASSETS : Singleton<ASSETS>
    {
        #region Asset References
    
        // Level Assets
        private LevelAssets _levels;
        public static LevelAssets LevelAssets => I._levels ? I._levels : Resources.Load<LevelAssets>("Assets/Levels");
        private GlobalVars _vars;
        public static GlobalVars Vars => I._vars ? I._vars : Resources.Load<GlobalVars>("Assets/GlobalVars");

        // Character/Player Assets
        private CharacterPrefabAssets _players;
        public static CharacterPrefabAssets Players => I._players ? I._players : Resources.Load<CharacterPrefabAssets>("Assets/Players");
    
        // UI Assets
        private UIAssets _ui;
        public static UIAssets ui => I._ui ? I._ui : Resources.Load<UIAssets>("Assets/UI");
    
        // FX Assets
        private FXAssets _fx;
        public static FXAssets FX {
            get {
                // Check if instance exists
                if (I == null) {
                    Debug.LogWarning("ASSETS.FX accessed before ASSETS singleton initialization");
                    return Resources.Load<FXAssets>("Assets/FX");
                }
            
                // Check if FX is loaded
                if (I._fx == null) {
                    I._fx = Resources.Load<FXAssets>("Assets/FX");
                    if (I._fx == null) {
                        Debug.LogError("Failed to load FX assets");
                    }
                }
            
                return I._fx;
            }
        }

        // House Assets
        private HouseAssets _house;
        public static HouseAssets House
        {
            get
            {
                // Check if instance exists
                if (I == null)
                {
                    Debug.LogWarning("ASSETS.FX accessed before ASSETS singleton initialization");
                    return Resources.Load<HouseAssets>("Assets/House");
                }

                // Check if HOUSE is loaded
                if (I._house == null)
                {
                    I._house = Resources.Load<HouseAssets>("Assets/House");
                    if (I._house == null)
                    {
                        Debug.LogError("Failed to load FX assets");
                    }
                }

                return I._house;
            }
        }

        // Scene Definition Assets
        private SceneDefinitionAssets _scenes;
        public static SceneDefinitionAssets Scenes => I._scenes ? I._scenes : Resources.Load<SceneDefinitionAssets>("Assets/SceneDefinitions");
    
        #endregion
    
        #region Lifecycle
    
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("ASSETS AWAKE");
            // Ensure we run in edit mode if needed
            if (Application.isEditor && !Application.isPlaying)
            {
                LoadAssets();
            }
        }
    
        private void Start()
        {
            LoadAssets();
        }
    
        /// <summary>
        /// Load all asset references
        /// </summary>
        private void LoadAssets()
        {
            // Check if we need to load/reload assets
            if (_levels == null || _players == null || _ui == null || _fx == null || _scenes == null)
            {
                StartCoroutine(LoadAssetsAsync());
            }
        }
    
        /// <summary>
        /// Load assets asynchronously to prevent blocking the main thread
        /// </summary>
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
    
        #endregion
    
        #region Scene Utility Methods
 
    
    
    
        #endregion
    }
}