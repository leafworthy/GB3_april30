using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Night
    {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightParticleSystem2D))]
    public class LightParticleSystem2DEditor : UnityEditor.Editor {

        override public void OnInspectorGUI() {
            LightParticleSystem2D script = target as LightParticleSystem2D;

            script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

            script.color = EditorGUILayout.ColorField("Color", script.color);
            
            script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

            script.scale = EditorGUILayout.FloatField("Scale", script.scale);

            script.useParticleColor = EditorGUILayout.Toggle("Use Particle Color", script.useParticleColor);

            script.customParticle = (Texture)EditorGUILayout.ObjectField("Custom Particle", script.customParticle, typeof(Texture), true);

            if (GUI.changed){
                if (EditorApplication.isPlaying == false) {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }
    }
}
