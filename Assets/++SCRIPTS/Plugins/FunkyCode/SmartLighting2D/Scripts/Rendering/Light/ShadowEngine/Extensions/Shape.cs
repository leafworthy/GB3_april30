﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightCollider2D;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2.Polygon2;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Light.ShadowEngine.Extensions
{
    public class Shape
    {
        public static void Draw(Light2D light, LightCollider2D lightCollider)
        {
            if (!lightCollider.InLight(light))
            {
                return;
            }

            if (light.eventPresetId > 0)
            {
                // optimize - only if event handling enabled
                // used to update light when light collider leaves light bounds
                light.AddCollider(lightCollider);
            }

            float shadowMin = lightCollider.shadowDistanceMin;
            float shadowMax = lightCollider.shadowDistanceMax;

            if (lightCollider.shadowDistance == LightCollider2D.ShadowDistance.Infinite)
            {
                shadowMin = 0;
                shadowMax = 0;
            }

            LightColliderShape shape = lightCollider.mainShape;

            List<Polygon2> polygons = shape.GetPolygonsWorld();

            ShadowEngine.Draw(polygons, shadowMin, shadowMax, lightCollider.shadowTranslucency);
        }
    }
}
