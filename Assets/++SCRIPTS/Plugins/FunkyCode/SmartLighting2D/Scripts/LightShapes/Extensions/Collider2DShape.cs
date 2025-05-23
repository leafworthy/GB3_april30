﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Triangulation;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2.Polygon2;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.LightShapes.Extensions
{		
	public class Collider2DShape : Base
	{
		public bool edgeCollider2D = false;
				
		public override List<MeshObject> GetMeshes()
		{
			if (Meshes == null)
			{
				List<Polygon2> polygons = GetPolygonsLocal();

				if (polygons.Count > 0)
				{
					Meshes = new List<MeshObject>();
					
					foreach(Polygon2 poly in polygons)
					{
						if (poly.points.Length < 3)
						{
							continue;
						}
						
						Mesh mesh = PolygonTriangulator2.Triangulate (poly, Vector2.zero, Vector2.zero, PolygonTriangulator2.Triangulation.Advanced);
						
						if (mesh)
						{							
							MeshObject meshObject = MeshObject.Get(mesh);

							if (meshObject != null)
							{
								Meshes.Add(meshObject);
							}
						}
					}
				}
			}

			return(Meshes);
		}

		public override List<Polygon2> GetPolygonsLocal()
		{
			if (LocalPolygons != null)
			{
				return(LocalPolygons);
			}

			if (transform == null)
			{
				return(LocalPolygons);
			}

			// avoid GC if possible
		
			LocalPolygons = Polygon2ListCollider2D.CreateFromGameObject(transform.gameObject);

			if (LocalPolygons.Count > 0)
			{
				edgeCollider2D = (transform.GetComponent<EdgeCollider2D>() != null);
			}
		
			return(LocalPolygons);
		}

		public override List<Polygon2> GetPolygonsWorld()
		{
			if (WorldPolygons != null)
			{
				return(WorldPolygons);
			}

			if (WorldCache != null)
			{
				WorldPolygons = WorldCache;

				Polygon2 poly;
				Polygon2 wPoly;
				
				List<Polygon2> list = GetPolygonsLocal();

				for(int i = 0; i < list.Count; i++)
				{
					poly = list[i];
					wPoly = WorldPolygons[i];

					for(int p = 0; p < poly.points.Length; p++)
					{
						wPoly.points[p] = poly.points[p];
					}

					wPoly.ToWorldSpaceSelfUNIVERSAL(transform);
				}

			}
				else
			{
				WorldPolygons = new List<Polygon2>();

				if (GetPolygonsLocal() != null)
				{
					foreach(Polygon2 poly in GetPolygonsLocal())
					{
						WorldPolygons.Add(poly.ToWorldSpace(transform));
					}
				}
		
				WorldCache = WorldPolygons;
			}
		
			return(WorldPolygons);
		}
	}
}
