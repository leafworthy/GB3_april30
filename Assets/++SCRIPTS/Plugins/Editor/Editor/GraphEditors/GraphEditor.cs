using __SCRIPTS.Plugins.AstarPathfindingProject.Core;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core.Misc;
using __SCRIPTS.Plugins.AstarPathfindingProject.Generators;
using __SCRIPTS.Plugins.AstarPathfindingProject.Utilities;
using __SCRIPTS.Plugins.Editor.ASTARPATH.Editor;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor.Editor.GraphEditors {
	public class GraphEditor : GraphEditorBase {
		public AstarPathEditor editor;

		/// <summary>Stores if the graph is visible or not in the inspector</summary>
		public FadeArea fadeArea;

		/// <summary>Stores if the graph info box is visible or not in the inspector</summary>
		public FadeArea infoFadeArea;

		/// <summary>
		/// Called by editor scripts to rescan the graphs e.g when the user moved a graph.
		/// Will only scan graphs if not playing and time to scan last graph was less than some constant (to avoid lag with large graphs)
		/// </summary>
		public bool AutoScan () {
			if (!Application.isPlaying && AstarPath.active != null && AstarPath.active.lastScanTime < 0.11F) {
				AstarPath.active.Scan();
				return true;
			}
			return false;
		}

		public virtual void OnEnable () {
		}

		public static Object ObjectField (string label, Object obj, System.Type objType, bool allowSceneObjects, bool assetsMustBeInResourcesFolder) {
			return ObjectField(new GUIContent(label), obj, objType, allowSceneObjects, assetsMustBeInResourcesFolder);
		}

		public static Object ObjectField (GUIContent label, Object obj, System.Type objType, bool allowSceneObjects, bool assetsMustBeInResourcesFolder) {
			obj = EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects);

			if (obj != null) {
				if (allowSceneObjects && !EditorUtility.IsPersistent(obj)) {
					// Object is in the scene
					var com = obj as Component;
					var go = obj as GameObject;
					if (com != null) {
						go = com.gameObject;
					}
					if (go != null && go.GetComponent<UnityReferenceHelper>() == null) {
						if (FixLabel("Object's GameObject must have a UnityReferenceHelper component attached")) {
							go.AddComponent<UnityReferenceHelper>();
						}
					}
				} else if (EditorUtility.IsPersistent(obj)) {
					if (assetsMustBeInResourcesFolder) {
						string path = AssetDatabase.GetAssetPath(obj).Replace("\\", "/");
						var rg = new System.Text.RegularExpressions.Regex(@"Resources/.*$");

						if (!rg.IsMatch(path)) {
							if (FixLabel("Object must be in the 'Resources' folder")) {
								if (!System.IO.Directory.Exists(Application.dataPath+"/Resources")) {
									System.IO.Directory.CreateDirectory(Application.dataPath+"/Resources");
									AssetDatabase.Refresh();
								}

								string ext = System.IO.Path.GetExtension(path);
								string error = AssetDatabase.MoveAsset(path, "Assets/Resources/"+obj.name+ext);

								if (error == "") {
									path = AssetDatabase.GetAssetPath(obj);
								} else {
									Debug.LogError("Couldn't move asset - "+error);
								}
							}
						}

						if (!AssetDatabase.IsMainAsset(obj) && obj.name != AssetDatabase.LoadMainAssetAtPath(path).name) {
							if (FixLabel("Due to technical reasons, the main asset must\nhave the same name as the referenced asset")) {
								string error = AssetDatabase.RenameAsset(path, obj.name);
								if (error != "") {
									Debug.LogError("Couldn't rename asset - "+error);
								}
							}
						}
					}
				}
			}

			return obj;
		}

		/// <summary>Draws common graph settings</summary>
		public void OnBaseInspectorGUI (NavGraph target) {
			int penalty = EditorGUILayout.IntField(new GUIContent("Initial Penalty", "Initial Penalty for nodes in this graph. Set during Scan."), (int)target.initialPenalty);

			if (penalty < 0) penalty = 0;
			target.initialPenalty = (uint)penalty;
		}

		/// <summary>Override to implement graph inspectors</summary>
		public virtual void OnInspectorGUI (NavGraph target) {
		}

		/// <summary>Override to implement scene GUI drawing for the graph</summary>
		public virtual void OnSceneGUI (NavGraph target) {
		}

		/// <summary>Draws a thin separator line</summary>
		public static void Separator () {
			GUIStyle separator = AstarPathEditor.astarSkin.FindStyle("PixelBox3Separator") ?? new GUIStyle();

			Rect r = GUILayoutUtility.GetRect(new GUIContent(), separator);

			if (Event.current.type == EventType.Repaint) {
				separator.Draw(r, false, false, false, false);
			}
		}

		/// <summary>Draws a small help box with a 'Fix' button to the right. Returns: Boolean - Returns true if the button was clicked</summary>
		public static bool FixLabel (string label, string buttonLabel = "Fix", int buttonWidth = 40) {
			GUILayout.BeginHorizontal();
			GUILayout.Space(14*EditorGUI.indentLevel);
			GUILayout.BeginHorizontal(AstarPathEditor.helpBox);
			GUILayout.Label(label, EditorGUIUtility.isProSkin ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
			var returnValue = GUILayout.Button(buttonLabel, EditorStyles.miniButton, GUILayout.Width(buttonWidth));
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			return returnValue;
		}

		/// <summary>Draws a toggle with a bold label to the right. Does not enable or disable GUI</summary>
		public bool ToggleGroup (string label, bool value) {
			return ToggleGroup(new GUIContent(label), value);
		}

		/// <summary>Draws a toggle with a bold label to the right. Does not enable or disable GUI</summary>
		public static bool ToggleGroup (GUIContent label, bool value) {
			GUILayout.BeginHorizontal();
			GUILayout.Space(13*EditorGUI.indentLevel);
			value = GUILayout.Toggle(value, "", GUILayout.Width(10));
			GUIStyle boxHeader = AstarPathEditor.astarSkin.FindStyle("CollisionHeader");
			if (GUILayout.Button(label, boxHeader, GUILayout.Width(100))) {
				value = !value;
			}

			GUILayout.EndHorizontal();
			return value;
		}
	}
}
