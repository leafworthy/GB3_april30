using __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using ColorSpace = __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.ColorSpace;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Settings
    {
    public class ProjectSettingsEditor {

        static public void Draw() {
            EditorGUI.BeginChangeCheck ();

            ProjectSettings mainProfile = Lighting2D.ProjectSettings;

            mainProfile.Profile = (Profile)EditorGUILayout.ObjectField("Default Profile", mainProfile.Profile, typeof(Profile), true);

            EditorGUILayout.Space();

            mainProfile.renderingMode = (RenderingMode)EditorGUILayout.EnumPopup("Rendering Mode", mainProfile.renderingMode);

            EditorGUILayout.Space();

            mainProfile.colorSpace = (ColorSpace)EditorGUILayout.EnumPopup("Color Space", mainProfile.colorSpace);

            EditorGUILayout.Space();

            mainProfile.managerInstance = (ManagerInstance)EditorGUILayout.EnumPopup("Manager Instance", mainProfile.managerInstance);

            mainProfile.managerInternal = (ManagerInternal)EditorGUILayout.EnumPopup("Manager Internal", mainProfile.managerInternal);

            EditorGUILayout.Space();

            mainProfile.MaxLightSize = EditorGUILayout.IntSlider("Max Light Size", mainProfile.MaxLightSize, 10, 1000);

            EditorGUILayout.Space();

            EditorView.Draw(mainProfile);

            EditorGUILayout.Space();

            Chunks.Draw(mainProfile);

            EditorGUI.EndChangeCheck ();

            if (GUI.changed) {
                LightingManager2D.ForceUpdate();
                Lighting2D.UpdateByProfile(mainProfile.Profile);

                EditorUtility.SetDirty(mainProfile);
            }
        }


        public class Chunks {
            public static void Draw(ProjectSettings mainProfile) {
                bool foldout = GUIFoldoutHeader.Begin("Chunks", mainProfile.chunks);

                if (foldout == false) {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                EditorGUILayout.Space();



                mainProfile.chunks.enabled = EditorGUILayout.Toggle("Enable", mainProfile.chunks.enabled);

                mainProfile.chunks.chunkSize = EditorGUILayout.IntSlider("Chunk Size", mainProfile.chunks.chunkSize, 10, 100);


                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }



        public class EditorView {
            public static void Draw(ProjectSettings mainProfile) {
                bool foldout = GUIFoldoutHeader.Begin("Editor View", mainProfile.editorView);

                if (foldout == false) {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                mainProfile.editorView.drawGizmos = (EditorDrawGizmos)EditorGUILayout.EnumPopup("Draw Gizmos", mainProfile.editorView.drawGizmos);

                mainProfile.editorView.drawGizmosShadowCasters = (EditorShadowCasters)EditorGUILayout.EnumPopup("Gizmos Shadow Casters", mainProfile.editorView.drawGizmosShadowCasters);

                mainProfile.editorView.drawGizmosBounds = (EditorGizmosBounds)EditorGUILayout.EnumPopup("Gizmos Bounds", mainProfile.editorView.drawGizmosBounds);

                mainProfile.editorView.drawGizmosChunks = (EditorChunks)EditorGUILayout.EnumPopup("Gizmos Chunks", mainProfile.editorView.drawGizmosChunks);

                mainProfile.editorView.drawIcons = (EditorIcons)EditorGUILayout.EnumPopup("Gizmos Icons", mainProfile.editorView.drawIcons);


                EditorGUILayout.Space();

                mainProfile.editorView.gameViewLayer = EditorGUILayout.LayerField("Game Layer (Default)", mainProfile.editorView.gameViewLayer);

                mainProfile.editorView.sceneViewLayer = EditorGUILayout.LayerField("Scene Layer (Default)", mainProfile.editorView.sceneViewLayer);

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }
    }
}
