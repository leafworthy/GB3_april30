﻿using __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Night
    {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightSprite2D))]
    public class LightSprite2DEditor : UnityEditor.Editor {
        LightSprite2D lightSprite2D;

        SerializedProperty lightLayer;
        SerializedProperty type;
        SerializedProperty spriteMode;

        SerializedProperty sprite;
        SerializedProperty color;
        SerializedProperty flipX;
        SerializedProperty flipY;

        SerializedProperty transform_scale;
        SerializedProperty transform_position;
        SerializedProperty transform_rotation;
        SerializedProperty transform_applyRotation;

        private void InitProperties() {
            lightLayer = serializedObject.FindProperty("lightLayer");
            type = serializedObject.FindProperty("type");
            spriteMode = serializedObject.FindProperty("spriteMode");

            sprite = serializedObject.FindProperty("sprite");

            color = serializedObject.FindProperty("color");

            flipX = serializedObject.FindProperty("flipX");
            flipY = serializedObject.FindProperty("flipY");

            transform_scale = serializedObject.FindProperty("lightSpriteTransform.scale");
            transform_position = serializedObject.FindProperty("lightSpriteTransform.position");
            transform_rotation = serializedObject.FindProperty("lightSpriteTransform.rotation");
            transform_applyRotation = serializedObject.FindProperty("lightSpriteTransform.applyRotation");
        }

        private void OnEnable(){
            lightSprite2D = target as LightSprite2D;

            InitProperties();
        }

        override public void OnInspectorGUI() {
            lightLayer.intValue = EditorGUILayout.Popup("Layer (Light)", lightLayer.intValue, Lighting2D.Profile.layers.lightLayers.GetNames());

            EditorGUILayout.PropertyField(type, new GUIContent ("Type"));

            EditorGUILayout.PropertyField(spriteMode, new GUIContent ("Sprite Mode"));

            DrawSpriteRenderer(lightSprite2D);

            DrawTransform(lightSprite2D);

            GUIMeshMode.Draw(serializedObject, lightSprite2D.meshMode);

            GUIGlowMode.Draw(lightSprite2D.glowMode);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed){
                if (EditorApplication.isPlaying == false) {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }

        void DrawSpriteRenderer(LightSprite2D script) {
            if (script.spriteMode == LightSprite2D.SpriteMode.Custom) {
                bool foldout0 = GUIFoldout.Draw("Sprite Renderer", script);

                if (foldout0) {
                    EditorGUI.indentLevel++;

                    sprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite.objectReferenceValue, typeof(Sprite), true);

                    DrawColor();

                    EditorGUILayout.PropertyField(flipX, new GUIContent ("Flip X"));
                    EditorGUILayout.PropertyField(flipY, new GUIContent ("Flip Y"));

                    EditorGUI.indentLevel--;
                }
            } else {
                DrawColor();
            }
        }

        void DrawColor() {
            Color colorValue = lightSprite2D.color;

            #if UNITY_2018_1_OR_NEWER
                if (Lighting2D.QualitySettings.HDR != HDR.Off) {
                    colorValue = EditorGUILayout.ColorField(new GUIContent("Color"), colorValue, true, true, true);
                } else {
                    colorValue = EditorGUILayout.ColorField("Color", colorValue);
                }
            #else
                colorValue = EditorGUILayout.ColorField("Color", colorValue);
            #endif

            colorValue.a = EditorGUILayout.Slider("Alpha", colorValue.a, 0, 1);

            color.colorValue = colorValue;
        }

        void DrawTransform(LightSprite2D script) {
            bool foldout = GUIFoldout.Draw("Transform", script.lightSpriteTransform);

            if (foldout) {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(transform_position, new GUIContent ("Position"));
                EditorGUILayout.PropertyField(transform_scale, new GUIContent ("Scale"));
                EditorGUILayout.PropertyField(transform_rotation, new GUIContent ("Rotation"));
                EditorGUILayout.PropertyField(transform_applyRotation, new GUIContent ("Apply Rotation"));

                EditorGUI.indentLevel--;
            }
        }
    }
}
