#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [InitializeOnLoad]
    public static class EditorWindowUtility
    {
        #region Variables
        
        public static ShaderAutoUpdater_SO shaderAutoUpdater;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Constructor

        static EditorWindowUtility()
        {
            Initialisation();
            EditorApplication.update += RunOnceOnStartup;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions

        private static void Initialisation()
        {
            shaderAutoUpdater = EditorExtensions.GetAssetByType<ShaderAutoUpdater_SO>();
        }
        
        private static void RunOnceOnStartup()
        {
            if (!shaderAutoUpdater) { Initialisation(); }
            if (SessionState.GetBool("EditorWindowUtilityFirstInit", false)) { return; }

            if (GaskellgamesSettings_SO.Instance && GaskellgamesSettings_SO.Instance.ShowHubOnStartup)
            {
                GaskellgamesHub.OpenWindow_WindowMenu();
            }

            if (shaderAutoUpdater)
            {
                shaderAutoUpdater.UpdateMaterialsForCurrentTargetPipeline();
            }
            
            SessionState.SetBool("EditorWindowUtilityFirstInit", true);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Public Functions

        public static Texture LoadInspectorBanner(string ggPathRef, string relativeFilepath)
        {
            if (GgPackageRef.TryGetFullFilePath(ggPathRef, relativeFilepath, out string fullFilepath))
            {
                return (Texture)AssetDatabase.LoadAssetAtPath(fullFilepath, typeof(Texture));
            }

            return null;
        }
        
        public static bool TryDrawBanner(Texture banner, bool editorWindow = false, bool forceShow = false)
        {
            // null and condition check
            if (!GaskellgamesSettings_SO.Instance) { return false; }
            if (!forceShow)
            {
                if (!GaskellgamesSettings_SO.Instance.ShowPackageBanners) { return false; }
                if (editorWindow && !GaskellgamesSettings_SO.Instance.ShowEditorWindowBanner) { return false; }
                if (!editorWindow && !GaskellgamesSettings_SO.Instance.ShowComponentBanner) { return false; }
            }
            if (!banner) { return false; }
            
            float imageWidth = EditorGUIUtility.currentViewWidth;
            float imageHeight = imageWidth * banner.height / banner.width;
            Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
            
            // adjust rect to account for offsets in inspectors
            if (!editorWindow)
            {
                float paddingTop = -4;
                float paddingLeft = -18;
                float paddingRight = -4;
                
                // calculate rect size
                float xMin = rect.x + paddingLeft;
                float yMin = rect.y + paddingTop;
                float width = rect.width - (paddingLeft + paddingRight);
                float height = rect.height;

                rect = new Rect(xMin, yMin, width, height);
            }
            
            GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
            return true;
        }

        #endregion
        
    } // class end
}

#endif