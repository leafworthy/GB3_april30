﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Triangulation;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2D;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2D.Polygon;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2.Polygon2
{
	public class Polygon2
	{
		public Vector2[] points;

		public Rect GetRect()
		{
			Rect rect = new Rect();

			float minX = 100000;
			float minY = 100000;
			float maxX = -100000;
			float maxY = -100000;

			int pointsCount = points.Length;

			for(int i = 0; i < pointsCount; i++)
			{
				Vector2 id = points[i];

				minX = (id.x < minX) ? id.x : minX;
				maxX = (id.x > maxX) ? id.x : maxX;

				minY = (id.y < minY) ? id.y : minY;
				maxY = (id.y > maxY) ? id.y : maxY;
			}

			rect.x = minX;
			rect.y = minY;
			rect.width = maxX - minX;
			rect.height = maxY - minY;

			return(rect);
		}

		public Polygon2 Copy()
		{
			Vector2[] array = new Vector2[points.Length];

			System.Array.Copy(points, array, points.Length);

			return(new Polygon2(array));
		}

		public Polygon2 ToWorldSpace(Transform transform)
		{
			Polygon2 newPolygon = this.Copy();

			newPolygon.ToWorldSpaceSelfUNIVERSAL(transform);

			return(newPolygon);
		}

		public void ToScaleSelf(Vector2 scale, Vector2? center = null)
		{
			if (center == null)
			{
				center = Vector2.zero;
			}

			float dist, rot;
			Vector2 point;

			for(int id = 0; id < points.Length; id++) {
				point = points[id];

				dist = Vector2.Distance(point, center.Value);
				rot = point.Atan2(center.Value); //??

				point.x = center.Value.x + Mathf.Cos(rot) * dist * scale.x;
				point.y = center.Value.y + Mathf.Sin(rot) * dist * scale.y;

				points[id] = point;
			}
		}

		public void ToRotationSelf(float rotation, Vector2? center = null)
		{
			if (center == null)
			{
				center = Vector2.zero;
			}

			float dist, rot;
			Vector2 point;

			for(int id = 0; id < points.Length; id++)
			{
				point = points[id];

				dist = Vector2.Distance(point, center.Value);
				rot = point.Atan2(center.Value) + rotation; //??

				point.x = center.Value.x + Mathf.Cos(rot) * dist;
				point.y = center.Value.y + Mathf.Sin(rot) * dist;

				points[id] = point;
			}
		}

		public Polygon2(List<Vector2> pointList)
		{
			points = pointList.ToArray();
		}

		public Polygon2(int size)
		{
			points = new Vector2[size];
		}

		public Polygon2(Polygon2D polygon)
		{
			points = new Vector2[polygon.pointsList.Count];

			for(int id = 0; id < polygon.pointsList.Count; id++) {
				points[id] = polygon.pointsList[id].ToVector2();
			}
		}

		public Polygon2(Vector2[] array)
		{
			points = array;
		}

		public void ToOffsetSelf(Vector2 pos)
		{
			for(int id = 0; id < points.Length; id++)
			{
				points[id] += pos;
			}
		}

		public bool IsClockwise()
		{
			if (points.Length < 1)
			{
				return (true);
			}

			double sum = 0;

			Vector2 A = points[points.Length - 1];
			Vector2 B;

			for(int i = 0; i < points.Length; i++) {
				B = points[i];

				sum += (B.x - A.x) * (B.y + A.y);

				A = B;
			}

			return(sum > 0);
		}

		public void Normalize()
		{
			if (!IsClockwise ())
			{
				System.Array.Reverse(points);
			}
		}






		///// Constructors - Polygon Creating //////

		static public Polygon2 CreateRect(Vector2 size) {
			size = size / 2;

			Polygon2 polygon = new Polygon2(4);

			polygon.points[0] = new Vector2(-size.x, -size.y);
			polygon.points[1] = new Vector2(size.x, -size.y);
			polygon.points[2] = new Vector2(size.x, size.y);
			polygon.points[3] = new Vector2(-size.x, size.y);

			polygon.Normalize();

			return(polygon);
		}

		static public Polygon2 CreateIsometric(Vector2 size) {
			size = size / 2;

			Polygon2 polygon = new Polygon2(4);

			polygon.points[0] = new Vector2(-size.x, size.y );
			polygon.points[1] = new Vector2(0, 0);
			polygon.points[2] = new Vector2(size.x,  size.y);
			polygon.points[3] = new Vector2(0, size.y * 2);

			polygon.Normalize();

			return(polygon);
		}

		static public Polygon2 CreateHexagon(Vector2 size) {
			size = size / 2;

			Polygon2 polygon = new Polygon2(6);


			polygon.points[0] = new Vector2(-size.x, size.y);
			polygon.points[1] = new Vector2(-size.x, -size.y);
			polygon.points[2] = new Vector2(0, -size.y * 2);
			polygon.points[3] = new Vector2(size.x, -size.y);
			polygon.points[4] = new Vector2(size.x,  size.y);
			polygon.points[5] = new Vector2(0, size.y * 2);

			polygon.Normalize();

			return(polygon);
		}

		public Mesh CreateMesh(GameObject gameObject, Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced) {
			if (gameObject.GetComponent<MeshRenderer>() == null) {
				gameObject.AddComponent<MeshRenderer>();
			}

			MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
			if (filter == null) {
				filter = gameObject.AddComponent<MeshFilter>() as MeshFilter;
			}

			filter.sharedMesh = PolygonTriangulator2.Triangulate (this, UVScale, UVOffset, triangulation);
			if (filter.sharedMesh == null) {
				UnityEngine.Object.Destroy(gameObject);
			}

			return(filter.sharedMesh);
		}

		public Mesh CreateMesh(Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced) {
			return(PolygonTriangulator2.Triangulate (this, UVScale, UVOffset, triangulation));
		}
































		public void ToWorldSpaceSelfUNIVERSAL(Transform transform)
		{
			switch(Lighting2D.CoreAxis)
			{
				case CoreAxis.XY:

					ToWorldSpaceSelfXY(transform);

				break;

				case CoreAxis.XYFLIPPED:

					ToWorldSpaceSelfFlipped(transform);

				break;

				case CoreAxis.XZFLIPPED:

					ToWorldSpaceSelfXZFlipped(transform);

				break;

				case CoreAxis.XZ:

					ToWorldSpaceSelfXZ(transform);

				break;
			}
		}

		//public void ToWorldSpaceSelf(Transform transform) {
		//	for(int id = 0; id < points.Length; id++) {
		//		points[id] = transform.TransformPoint (points[id]);
		//	}
		//}

		public void ToWorldSpaceSelfXY(Transform transform)
		{
			int count = points.Length;

			Vector2 scale = transform.lossyScale;
			Vector2 position = transform.position;
			float rotation = transform.eulerAngles.z;

			float angle = rotation * Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);

			for(int id = 0; id < count; id++)
			{
				Vector2 a = points[id];

				float x = a.x * scale.x;
				float y = a.y * scale.y;

				a.x = x * cos - y * sin + position.x;
				a.y = x * sin + y * cos + position.y;

				points[id] = a;
			}
		}

		public void ToWorldSpaceSelfFlipped(Transform transform) {
			int count = points.Length;

			for(int id = 0; id < count; id++) {
				points[id] = points[id].TransformToWorldXYFlipped(transform);
			}
		}

		public void ToWorldSpaceSelfXZ(Transform transform) {
			int count = points.Length;

			for(int id = 0; id < count; id++) {
				points[id] = points[id].TransformToWorldXZ(transform);
			}
		}

		public void ToWorldSpaceSelfXZFlipped(Transform transform) {
			int count = points.Length;

			for(int id = 0; id < count; id++) {
				points[id] = points[id].TransformToWorldXZFlipped(transform);
			}
		}

		public bool PointInPoly(Vector2 point)
		{
			return(Math2D.PointInPoly(point, this));
		}
	}
}
