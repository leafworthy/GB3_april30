using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
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





	}
}
