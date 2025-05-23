﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2.Polygon2;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Camera
{
    public class CameraTransform
    {
        public static List<CameraTransform> List = new List<CameraTransform>();

        private UnityEngine.Camera camera = null;
        private Transform transform = null;

        private Polygon2 worldPolygon = null;
        private Rect worldRect = new Rect();

        private Polygon2 polygon = null;

        private Vector2 position = Vector2.zero;
        private float rotation = 0;
        private Vector2 scale = Vector2.one;
        private float size = 0;

        // Change
        public static float GetRadius(UnityEngine.Camera camera)
        {
            float cameraRadius = camera.orthographicSize;

            if (camera.pixelWidth > camera.pixelHeight)
            {
                cameraRadius *= (float)camera.pixelWidth / camera.pixelHeight;
            }

            cameraRadius = Mathf.Sqrt(cameraRadius * cameraRadius + cameraRadius * cameraRadius);

            return(cameraRadius);
        }

        public static Rect GetWorldRect(UnityEngine.Camera camera)
        {
            CameraTransform cameraTransform = GetCamera(camera);

            return(cameraTransform.WorldRect());
        }

        public static CameraTransform GetCamera(UnityEngine.Camera camera)
        {
            if (camera == null)
            {
                UnityEngine.Debug.LogError("Camera == Null");
            }

            foreach(CameraTransform transform in List)
            {
                if (transform.camera == camera)
                {
                    return(transform);
                }
            }

            CameraTransform cameraTransform = new CameraTransform();
            cameraTransform.camera = camera;
            cameraTransform.transform = camera.transform;

            cameraTransform.Update(true);

            List.Add(cameraTransform);

            return(cameraTransform);
        }

        public void Update(bool force = false)
        {
            if (camera == null)
            {
                return;
            }
            
            Transform transform = camera.transform;

            if (transform.hasChanged || force)
            {
                transform.hasChanged = false;

                position = LightingPosition.GetPosition2D(transform.position);
                scale = transform.lossyScale;
                rotation = LightingPosition.GetRotation2D(transform);
                size = camera.orthographicSize;

                worldPolygon = null;
            }
        }

        private Rect WorldRect()
        {
            if (worldPolygon != null)
            {
                return(worldRect);
            }

            return(WorldRectGenerate());
        }

        private Rect WorldRectGenerate()
        {
            float cameraSizeY = camera.orthographicSize;
            float cameraSizeX = cameraSizeY * (float)camera.pixelWidth / camera.pixelHeight;

            float sizeX = cameraSizeX * 2;
            float sizeY = cameraSizeY * 2;

            float x = -sizeX / 2;
            float y = -sizeY / 2;

            worldPolygon = Polygon();

            worldPolygon.points[0].x = x;
            worldPolygon.points[0].y = y;

            worldPolygon.points[1].x = x + sizeX;
            worldPolygon.points[1].y = y;

            worldPolygon.points[2].x = x + sizeX;
            worldPolygon.points[2].y = y + sizeY;

            worldPolygon.points[3].x = x;
            worldPolygon.points[3].y = y + sizeY;

            worldPolygon.ToRotationSelf(rotation * Mathf.Deg2Rad);
            worldPolygon.ToOffsetSelf(position);

            worldRect = worldPolygon.GetRect();

            return(worldRect);
        }
        
        private Polygon2 Polygon()
        {
            if (polygon == null)
            {
                polygon = new Polygon2(4);
                
                polygon.points[0] = Vector2.zero;
                polygon.points[1] = Vector2.zero;
                polygon.points[2] = Vector2.zero;
                polygon.points[3] = Vector2.zero;
            }

            return(polygon);
        }
    }
}
