﻿using System;
using System.Collections.Generic;
using System.Reflection;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using SortingLayer = __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.SortingLayer;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc
{
	public class GUIFoldout
	{
		static Dictionary<object, bool> dictionary = new Dictionary<object, bool>();

		static public bool GetValue(object Object)
		{
			bool value = false;

			if (!dictionary.TryGetValue(Object, out value))
			{
				dictionary.Add(Object, value);
			}

			return(value);
		}

		static public void SetValue(object Object, bool value)
		{
			bool resultVal;

			if (dictionary.TryGetValue(Object, out resultVal))
			{
				dictionary.Remove(Object);
				dictionary.Add(Object, value);
			}
		}

		static public bool Draw(string name, object Object)
		{
			bool value = EditorGUILayout.Foldout(GetValue(Object), name);

			SetValue(Object, value);

			return(value);
		}
	}

	public class GUIFoldoutHeader
	{
		static Dictionary<object, bool> dictionary = new Dictionary<object, bool>();

		static public bool GetValue(object Object)
		{
			bool value = false;

			if (!dictionary.TryGetValue(Object, out value))
			{
				dictionary.Add(Object, value);
			}

			return(value);
		}

		static public void SetValue(object Object, bool value)
		{
			bool resultVal;

			if (dictionary.TryGetValue(Object, out resultVal)) {
				dictionary.Remove(Object);
				dictionary.Add(Object, value);
			}
		}

		static public bool Begin(string name, object Object)
		{
			#if UNITY_2019_1_OR_NEWER
				bool value = EditorGUILayout.BeginFoldoutHeaderGroup(GetValue(Object), name);
			#else
				bool value = EditorGUILayout.Foldout(GetValue(Object), name);
			#endif

			SetValue(Object, value);

			return(value);
		}

		static public void End()
		{
			#if UNITY_2019_1_OR_NEWER
				EditorGUILayout.EndFoldoutHeaderGroup();
			#endif
		}
	}

	public class GUISortingLayer
	{
		static public string[] GetSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
	
		static public int[] GetSortingLayerUniqueIDs()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
			
			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}

		static public void Draw(SerializedObject serializedObject, SortingLayer sortingLayer, string serializationDepth = "")
		{
			bool value = GUIFoldout.Draw("Sorting Layer", sortingLayer);
			
			if (!value)
			{
				return;
			}

			SerializedProperty order = serializedObject.FindProperty(serializationDepth + "sortingLayer.Order");
			SerializedProperty name = serializedObject.FindProperty(serializationDepth + "sortingLayer.name");

			EditorGUI.indentLevel++;

				string[] sortingLayerNames = GetSortingLayerNames();
				int id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
				int newId = EditorGUILayout.Popup("Name", id, sortingLayerNames);

				if (newId > -1 && newId < sortingLayerNames.Length)
				{
					string newName = sortingLayerNames[newId];

					if (newName != sortingLayer.Name)
					{
						name.stringValue = newName;
					}
				}
				
				EditorGUILayout.PropertyField(order, new GUIContent ("Order"));
		
			EditorGUI.indentLevel--;
		}

		static public void Draw(SortingLayer sortingLayer, bool drawFoldout)
		{
			if (drawFoldout)
			{
				bool value = GUIFoldout.Draw("Sorting Layer", sortingLayer);
			
				if (!value)
				{
					return;
				}

				EditorGUI.indentLevel++;
			}
			
			string[] sortingLayerNames = GetSortingLayerNames();
			int id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
			int newId = EditorGUILayout.Popup("Sorting Layer", id, sortingLayerNames);

			if (newId > -1 && newId < sortingLayerNames.Length)
			{
				string newName = sortingLayerNames[newId];

				if (newName != sortingLayer.Name)
				{
					sortingLayer.Name = newName;
				}
			}
			
			sortingLayer.Order = EditorGUILayout.IntField("Order in Layer", sortingLayer.Order);

			if (drawFoldout)
			{
				EditorGUI.indentLevel--;
			}
		}
	}

	public class GUIMeshMode
	{
		public static void Draw(SerializedObject serializedObject, MeshMode meshMode)
		{
			bool value = GUIFoldout.Draw("Mesh Mode", meshMode);
			
			if (!value)
			{
				return;
			}

			EditorGUI.indentLevel++;

			SerializedProperty meshModeEnable = serializedObject.FindProperty("meshMode.enable");
			SerializedProperty meshModeAlpha = serializedObject.FindProperty("meshMode.alpha");
			SerializedProperty meshModeShader =serializedObject.FindProperty("meshMode.shader");
			
			EditorGUILayout.PropertyField(meshModeEnable, new GUIContent ("Enable"));

			meshModeAlpha.floatValue = EditorGUILayout.Slider("Alpha", meshModeAlpha.floatValue, 0, 1);

			EditorGUILayout.PropertyField(meshModeShader, new GUIContent ("Shader"));

			if (meshModeShader.intValue == (int)MeshModeShader.Custom)
			{
				bool value2 = GUIFoldout.Draw("Materials", meshMode.materials);

				if (value2)
				{
					EditorGUI.indentLevel++;

					int count = meshMode.materials.Length;
					count = EditorGUILayout.IntSlider("Material Count", count, 0, 10);

					if (count != meshMode.materials.Length)
					{
						System.Array.Resize(ref meshMode.materials, count);
					}

					for(int id = 0; id < meshMode.materials.Length; id++)
					{
						UnityEngine.Material material = meshMode.materials[id];

						material = (UnityEngine.Material)EditorGUILayout.ObjectField("Material", material, typeof(UnityEngine.Material), true);

						meshMode.materials[id] = material;
					}

					EditorGUI.indentLevel--;
				}
			}

			GUISortingLayer.Draw(serializedObject, meshMode.sortingLayer, "meshMode.");

			EditorGUI.indentLevel--;
		}
	}

	public class GUIBumpMapMode
	{
		static public void Draw (SerializedObject serializedObject, object obj)
		{ 
			// Serialized property
			bool value = GUIFoldout.Draw("Mask Bump Map", obj);
			
			if (!value) {
				return;
			}

			EditorGUI.indentLevel++;

			SerializedProperty bumpType = serializedObject.FindProperty("bumpMapMode.type");
			SerializedProperty bumpTextureType = serializedObject.FindProperty("bumpMapMode.textureType");
			SerializedProperty bumpTexture = serializedObject.FindProperty("bumpMapMode.texture");
			SerializedProperty bumpSprite = serializedObject.FindProperty("bumpMapMode.sprite");

			SerializedProperty invertX = serializedObject.FindProperty("bumpMapMode.invertX");
			SerializedProperty invertY = serializedObject.FindProperty("bumpMapMode.invertY");

			SerializedProperty depth = serializedObject.FindProperty("bumpMapMode.depth");

			SerializedProperty spriteRenderer = serializedObject.FindProperty("bumpMapMode.spriteRenderer");
			SpriteRenderer sr = (SpriteRenderer)spriteRenderer.objectReferenceValue;

			EditorGUILayout.PropertyField(bumpType, new GUIContent ("Type"));
			EditorGUILayout.PropertyField(bumpTextureType, new GUIContent ("Texture Type"));

			EditorGUILayout.PropertyField(invertX, new GUIContent ("Invert X"));

			EditorGUILayout.PropertyField(invertY, new GUIContent ("Invert Y"));

			EditorGUILayout.PropertyField(depth, new GUIContent ("Depth"));

			switch(bumpTextureType.intValue)
			{
				case (int)NormalMapTextureType.Texture:

					bumpTexture.objectReferenceValue = (Texture)EditorGUILayout.ObjectField("Texture", bumpTexture.objectReferenceValue, typeof(Texture), true);

				break;

				case (int)NormalMapTextureType.Sprite:

					bumpSprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Sprite", bumpSprite.objectReferenceValue, typeof(Sprite), true);

				break;

				case (int)NormalMapTextureType.SecondaryTexture:

					MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
					sr.GetPropertyBlock(matBlock);
					Texture secondaryTexture = matBlock.GetTexture("_SecondaryTex");

					EditorGUI.BeginDisabledGroup(true);

					EditorGUILayout.ObjectField("Sprite", secondaryTexture, typeof(Sprite), true);

					EditorGUI.EndDisabledGroup();

				break;
			}

			EditorGUI.indentLevel--;
		}

		static public void DrawDay (DayNormalMapMode bumpMapMode)
		{
			bool value = GUIFoldout.Draw("Mask Normal Map", bumpMapMode);
			
			if (!value)
			{
				return;
			}

			EditorGUI.indentLevel++;

			bumpMapMode.textureType = (NormalMapTextureType)EditorGUILayout.EnumPopup("Texture Type", bumpMapMode.textureType);

			switch(bumpMapMode.textureType)
			{
				case NormalMapTextureType.Texture:

					bumpMapMode.texture = (Texture)EditorGUILayout.ObjectField("Texture", bumpMapMode.texture, typeof(Texture), true);

				break;

				case NormalMapTextureType.Sprite:

					bumpMapMode.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", bumpMapMode.sprite, typeof(Sprite), true);

				break;
			}

			EditorGUI.indentLevel--;
		}
	}

	public class GUIGlowMode
	{
		static public void Draw(GlowMode glowMode)
		{
			bool value = GUIFoldout.Draw("Glow Mode", glowMode);
			
			if (!value) {
				return;
			}

			EditorGUI.indentLevel++;

			glowMode.enable = EditorGUILayout.Toggle("Enable", glowMode.enable);

			glowMode.glowRadius = EditorGUILayout.Slider("Glow Size", glowMode.glowRadius, 0.1f, 10);

			EditorGUI.indentLevel--;
		}
	}
}
