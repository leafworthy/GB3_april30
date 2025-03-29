using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor
{
    [InitializeOnLoad]
    public static class IsoSortingEditorUpdater
    {
        private static double lastUpdateTime;
        private static readonly double updateInterval = 0.5; // Update every half second

        static IsoSortingEditorUpdater()
        {
            EditorApplication.update += OnEditorUpdate;
            SceneView.duringSceneGui += OnSceneGUI;
        
            // Subscribe to scene change events
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        private static void OnEditorUpdate()
        {
            // Update sorting at regular intervals
            if (EditorApplication.timeSinceStartup - lastUpdateTime > updateInterval)
            {
                lastUpdateTime = EditorApplication.timeSinceStartup;
                UpdateSorting();
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            // Update sorting when interacting with the scene view
            if (Event.current.type == EventType.MouseDrag || 
                Event.current.type == EventType.MouseDown || 
                Event.current.type == EventType.MouseUp)
            {
                // Delay the update slightly to allow transform changes to complete
                // EditorApplication.delayCall += UpdateSorting;
            }
        }

        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            // Force update when a scene is opened
            //EditorApplication.delayCall += UpdateSorting;
        }

        private static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
        {
            // Force update when a scene is saved
            EditorApplication.delayCall += UpdateSorting;
        }

        private static void UpdateSorting()
        {
            if (!Application.isPlaying)
            {
                // IsoSpriteSorting.UpdateSorters();
            }
        }
    }
}