﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Day;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Day.Masks;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Day.Shadow;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Day
{
    public static class NoSort
    {
        static public void Draw(Pass pass)
        {
            bool drawShadows = pass.layer.type != LayerType.MaskOnly;
            bool drawMask = pass.layer.type != LayerType.ShadowsOnly;

            if (drawShadows)
            {
                Day.Shadow.Shadow.Begin();

                Shadow.DrawCollider(pass);
                Shadow.DrawTilemapCollider(pass);

                Day.Shadow.Shadow.End();

                Shadow.DrawColliderFill(pass);

                Shadow.DrawSprite(pass);
            }

            if (drawMask)
            {
                Mask.DrawCollider(pass);

                Mask.DrawTilemap(pass);
            }
        }

        public static class Mask
        {
            public static void DrawCollider(Pass pass)
            {
                if (pass.colliderCount <= 0)
                {
                    return;
                }

                // regular sprites

                SpriteRenderer2D.currentTexture = null;

                for(int i = 0; i < pass.colliderCount; i++)
                {
                    DayLightCollider2D id = pass.colliderList[i];

                    if (id.maskLayer != pass.layerId)
                    {
                        continue;
                    }

                    switch(id.mainShape.maskType)
                    {
                        case DayLightCollider2D.MaskType.Sprite:

                            SpriteRenderer2D.Draw(id, pass.offset);

                        break;
                    }
                }

                if (SpriteRenderer2D.currentTexture != null)
                {
                    GL.End();

                    SpriteRenderer2D.currentTexture = null;
                }

                // bumped sprites

                for(int i = 0; i < pass.colliderCount; i++)
                {
                    DayLightCollider2D id = pass.colliderList[i];

                    if (id.maskLayer != pass.layerId)
                    {
                        continue;
                    }

                    switch(id.mainShape.maskType)
                    {
                        case DayLightCollider2D.MaskType.BumpedSprite:

                            SpriteRenderer2D.DrawBumped(id, pass.offset);

                        break;
                    }
                }
            }

            public static void DrawTilemap(Pass pass)
            {
                if (pass.tilemapColliderCount <= 0)
                {
                    return;
                }

                for(int i = 0; i < pass.tilemapColliderCount; i++)
                {
                    DayLightTilemapCollider2D id = pass.tilemapColliderList[i];

                    if (id.maskLayer != pass.layerId)
                    {
                        continue;
                    }

                    SpriteRenderer2D.DrawTilemap(id, pass.offset);
                }
            }
        }

        public static class Shadow
        {
            public static void DrawColliderFill(Pass pass)
            {
                if (pass.colliderCount <= 0)
                {
                    return;
                }

                Lighting2D.materials.shadow.GetDayCPUShadow().SetPass(0);
                GL.Begin(GL.TRIANGLES);

                for(int i = 0; i < pass.colliderCount; i++)
                {
                    DayLightCollider2D id = pass.colliderList[i];

                    if (id.shadowLayer != pass.layerId)
                    {
                        continue;
                    }

                    switch(id.mainShape.shadowType)
                    {
                        case DayLightCollider2D.ShadowType.FillCollider2D:
                        case DayLightCollider2D.ShadowType.FillSpritePhysicsShape:

                            Day.Shadow.Shadow.DrawFill(id, pass.offset);

                        break;
                    }
                }

                GL.End();
            }

            public static void DrawCollider(Pass pass)
            {
                if (pass.colliderCount <= 0)
                {
                    return;
                }

                for(int i = 0; i < pass.colliderCount; i++)
                {
                    DayLightCollider2D id = pass.colliderList[i];

                    if (id.shadowLayer != pass.layerId)
                    {
                        continue;
                    }

                    switch(id.mainShape.shadowType)
                    {
                        case DayLightCollider2D.ShadowType.SpritePhysicsShape:
                        case DayLightCollider2D.ShadowType.Collider2D:

                            Day.Shadow.Shadow.Draw(id, pass.offset);

                        break;
                    }
                }
            }

            public static void DrawTilemapCollider(Pass pass)
            {
                if (pass.tilemapColliderCount <= 0)
                {
                    return;
                }

                for(int i = 0; i < pass.tilemapColliderCount; i++)
                {
                    DayLightTilemapCollider2D id = pass.tilemapColliderList[i];

                    if (id.shadowLayer != pass.layerId)
                    {
                        continue;
                    }

                    Day.Shadow.Shadow.DrawTilemap(id, pass.offset);
                }
            }

            public static void DrawSprite(Pass pass)
            {
                if (pass.colliderCount <= 0)
                {
                    return;
                }

                SpriteRendererShadow.Begin(pass.offset);

                for(int i = 0; i < pass.colliderCount; i++)
                {
                    DayLightCollider2D id = pass.colliderList[i];

                    if (id.shadowLayer != pass.layerId)
                    {
                        continue;
                    }

                    switch(id.mainShape.shadowType)
                    {
                        case DayLightCollider2D.ShadowType.SpriteProjection:

                            SpriteRendererShadow.DrawProjection(id);

                        break;

                        case DayLightCollider2D.ShadowType.SpriteProjectionShape:
                        case DayLightCollider2D.ShadowType.SpriteProjectionCollider:

                            SpriteRendererShadow.DrawProjectionShape(id);

                        break;

                        case DayLightCollider2D.ShadowType.SpriteOffset:

                            SpriteRendererShadow.DrawOffset(id);

                        break;
                    }
                }

                if (SpriteRendererShadow.currentTexture != null)
                {
                    SpriteRendererShadow.End();
                }
            }
        }
    }
}
