﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;
using Texture = __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects.Texture;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Lightmap.Objects
{
	public class LightSource
    {
       static public void Draw(Light2D light, UnityEngine.Camera camera)
       {
            if (light.Buffer == null)
            {
                return;
            }

            if (!light.isActiveAndEnabled)
            {
                return;
            }

            if (!light.InCameras())
            {
                return;
            }

            if (!light.drawingEnabled)
            {
                return;
            }

            Vector2 pos = LightingPosition.GetPosition2D(-camera.transform.position);
            Vector2 size = new Vector2(light.size, light.size);

            if (light.IsPixelPerfect())
            {
                size = LightingRender2D.GetSize(camera);
                pos = Vector2.zero;
            }
                else
            {
                pos += light.transform2D.position;
            }
         
            Color lightColor = light.color;
            lightColor.a = light.color.a / 2;

            UnityEngine.Material material = null;

            switch(light.lightType)
            {
                case Light2D.LightType.Sprite:

                    material = Lighting2D.materials.GetSpriteLight();
                    material.mainTexture = light.Buffer.renderTexture.renderTexture;

                    material.SetTexture("_Sprite", light.GetSprite().texture);
                    material.SetFloat("_Rotation", light.transform2D.rotation * 0.0174533f);
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);

                break;

                case Light2D.LightType.FreeForm:

                    material = Lighting2D.materials.GetFreeFormLight();
                    material.mainTexture = light.Buffer.renderTexture.renderTexture;

                    material.SetTexture("_Sprite", light.Buffer.freeFormTexture.renderTexture);
                    material.SetFloat("_Point", light.freeFormPoint);

                break;

                case Light2D.LightType.Point:

                    material = Lighting2D.materials.GetPointLight();
                    material.mainTexture = light.Buffer.renderTexture.renderTexture;

                    material.SetFloat("_Strength", light.lightStrength);
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);
                    material.SetFloat("_Rotation", light.transform2D.rotation * 0.0174533f);

                break;
            }

            GLExtended.color = lightColor;

            Texture.Quad.Draw(material, pos, size, 0, 0);
        }

        static public void DrawOcclusion(Light2D light, UnityEngine.Camera camera)
        {
            if (light.Buffer == null)
            {
                return;
            }

            if (!light.isActiveAndEnabled)
            {
                return;
            }

            if (!light.InCameras())
            {
                return;
            }

            if (!light.drawingEnabled)
            {
                return;
            }

            Vector2 pos = LightingPosition.GetPosition2D(-camera.transform.position);
            Vector2 size = new Vector2(light.size, light.size);

            pos += light.transform2D.position;
         
            Color lightColor = light.color;
            lightColor.a = light.color.a / 2;

            UnityEngine.Material material = null;

            switch(light.lightType)
            {
                case Light2D.LightType.Sprite:

                    material = Lighting2D.materials.GetLightOcclusion();
                    material.mainTexture = light.GetSprite().texture;
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);

                break;

                case Light2D.LightType.FreeForm:

                    material = Lighting2D.materials.GetFreeFormOcclusion();
                    material.mainTexture = light.Buffer.freeFormTexture.renderTexture;
                    material.SetFloat("_Point", light.freeFormPoint);

                break;

                case Light2D.LightType.Point:

                    material = Lighting2D.materials.GetPointOcclusion();
                    material.SetFloat("_Strength", light.lightStrength);
                    material.SetFloat("_Rotation", 0); // light.transform2D.rotation * 0.0174533f
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);

                break;
            }
            
            GLExtended.color = lightColor;

            Texture.Quad.Draw(material, pos, size, light.transform2D.rotation, 0);
        }

        static public void DrawTranslucent(Light2D light, UnityEngine.Camera camera)
        {
            if (light.Buffer == null)
            {
                return;
            }

            if (!light.isActiveAndEnabled)
            {
                return;
            }

            if (!light.InCameras())
            {
                return;
            }

            if (!light.drawingTranslucencyEnabled)
            {
                return;
            }

            if (light.Buffer.translucencyTexture == null)
            {
                return;
            }

            Vector2 pos = LightingPosition.GetPosition2D(-camera.transform.position);
            Vector2 size = new Vector2(light.size, light.size);

            if (light.IsPixelPerfect())
            {
                size = LightingRender2D.GetSize(camera);
                pos = Vector2.zero;
            }
                else
            {
                pos += light.transform2D.position;
            }
         
            Color lightColor = light.color;
            lightColor.a = light.color.a / 2;

            UnityEngine.Material material = null;

            switch(light.lightType)
            {
                case Light2D.LightType.Sprite:

                    material = Lighting2D.materials.GetSpriteLight();
                    material.mainTexture = light.Buffer.translucencyTexture.renderTexture;

                    material.SetTexture("_Sprite", light.GetSprite().texture);
                    material.SetFloat("_Rotation", light.transform2D.rotation * 0.0174533f);
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);

                break;

                case Light2D.LightType.FreeForm:

                    material = Lighting2D.materials.GetFreeFormLight();
                    material.mainTexture = light.Buffer.translucencyTexture.renderTexture;

                    material.SetTexture("_Sprite", light.Buffer.freeFormTexture.renderTexture);
                    material.SetFloat("_Point", light.freeFormPoint);

                break;

                case Light2D.LightType.Point:

                    material = Lighting2D.materials.GetPointLight();
                    material.mainTexture = light.Buffer.translucencyTexture.renderTexture;

                    material.SetFloat("_Strength", light.lightStrength);
                    material.SetFloat("_Rotation", light.transform2D.rotation * 0.0174533f);
                    material.SetFloat("_Outer", light.spotAngleOuter - light.spotAngleInner);
                    material.SetFloat("_Inner", light.spotAngleInner);

                break;
            }

            GLExtended.color = lightColor;

            Texture.Quad.Draw(material, pos, size, 0, 0);
        }
    }
}
