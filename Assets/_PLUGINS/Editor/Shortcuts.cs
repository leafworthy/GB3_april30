using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Shortcuts
{
	public static Vector3 GetRandomDir()
	{
		return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
	}

	public static float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}
	[MenuItem("MyMenu/Edit Polygon Collider")]
	static void edit_polygon_collider()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (var assembly in assemblies)
		{
			if (assembly.GetType("UnityEditor.PolygonCollider2DTool") != null)
			{
				UnityEditor.EditorTools.ToolManager.SetActiveTool(
					assembly.GetType("UnityEditor.PolygonCollider2DTool"));
			}
		}
	}

	[MenuItem("MyMenu/Convert to DamageObject")]
	static void convertToDamageObject()
	{
		if (Selection.activeTransform != null)
		{
			Selection.activeTransform.gameObject.name = "DamageObject";
			Selection.activeTransform.gameObject.AddComponent<HideRevealObjects>().GatherTransforms();
		}
	}

	[MenuItem("MyMenu/Create DamageObject")]
	static void createDamageObject()
	{
		GameObject go = new GameObject("DamageObject");
		go.AddComponent<HideRevealObjects>();
		if (Selection.activeTransform != null)
		{
			foreach (Transform transform in Selection.activeTransform)
			{
				for (int i = 0; i < Selection.activeTransform.childCount; i++)
				{
					transform.parent = go.transform;
				}
			}

			go.transform.parent = Selection.activeTransform;
			go.transform.localPosition = Vector3.zero;
		}
	}

	[MenuItem("MyMenu/Change Pivot Point")]
	static void ChangePivotPoint()
	{
		Transform newPivot = null;
		Transform objectToChange = null;
		foreach (Transform t in Selection.transforms)
		{
			if (t.gameObject.CompareTag("pivot"))
			{
				Debug.Log("new pivot");
				newPivot = t;
			}
			else
			{
				Debug.Log("object to change");
				objectToChange = t;
			}
		}

		if (newPivot == null)
		{
			Debug.Log("no pivot");
			return;
		}

		GameObject temporaryHolder = new GameObject();
		var children = new List<Transform>();
		foreach (Transform transform in objectToChange.transform)
		{
			children.Add(transform);
		}

		foreach (Transform transform in children)
		{
			transform.parent = temporaryHolder.transform;
		}

		objectToChange.position = newPivot.transform.position;
		foreach (Transform transform in children)
		{
			transform.parent = objectToChange;
		}

		GameObject.DestroyImmediate(temporaryHolder);
	}




	[MenuItem("MyMenu/Convert To Destroyable Object", priority = 0)]
	private static void ConvertToDestroyableObject()
	{
		//REPARENT SELECTION
		Transform[] selection = Selection.transforms;
		if (selection.Length == 0)
			return;

		var originalParent = selection[0].parent;
		Transform damageObject = new GameObject().transform;
		damageObject.parent = originalParent;

		Undo.RegisterCreatedObjectUndo(damageObject.gameObject, "Parent Selected");
		for (int i = 0; i < selection.Length; i++)
		{
			if (AssetDatabase.Contains(selection[i].gameObject))
				continue;

			Undo.SetTransformParent(selection[i], damageObject, "Parent Selected");
		}


		damageObject.name = "Damage Object";
		damageObject.gameObject.AddComponent<HideRevealObjects>().GatherTransforms();
		Transform mainObject = new GameObject().transform;

		mainObject.name = "New Object";
		mainObject.parent = originalParent;
		damageObject.parent = mainObject;
		mainObject.gameObject.AddComponent<ObjectDamageFX>();
		mainObject.gameObject.AddComponent<DefenceHandler>();
		mainObject.gameObject.AddComponent<PolygonCollider2D>();
		Selection.activeTransform = mainObject;
	}
}
