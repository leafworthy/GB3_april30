﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Lightmap.Objects;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Lightmap
{
    public class NoSort
    {
        public static void Draw(Pass pass)
        {
            // Rooms
            DrawRooms(pass);

            DrawTilemapRooms(pass);

            // Light Emissions
            DrawLightSprites(pass);

            DrawLightSprites_Scriptable(pass);

            DrawLightTextures(pass);

            DrawLightParticleSystem(pass);

            // Light Sources

            DrawLight(pass);
        }

        private static void DrawRooms(Pass pass)
        {
            List<LightRoom2D> roomList = LightRoom2D.List;
            int roomCount = roomList.Count;

            if (roomCount < 1)
            {
                return;
            }

            // Draw Collider Pass
            Room.drawColliderPass = false;

            for(int i = 0; i < roomCount; i++)
            {
                LightRoom2D id = roomList[i];

                if (id.lightLayer != pass.layerId)
                {
                    continue;
                }

                switch(id.shape.type)
                {
                    case LightRoom2D.RoomType.Collider:
                        Room.DrawColliderPass(id, pass.camera);
                    break;
                }
            }

            if (Room.drawColliderPass)
            {
                GL.End();
            }

            // Draw Sprite Pass?
            for(int i = 0; i < roomCount; i++)
            {
                LightRoom2D id = roomList[i];

                if (id.lightLayer != pass.layerId)
                {
                    continue;
                }

                switch(id.shape.type)
                {
                    case LightRoom2D.RoomType.Sprite:

                        Room.DrawSprite(id, pass.camera);

                    break;
                }
            }
        }

        private static void DrawTilemapRooms(Pass pass)
        {
            List<LightTilemapRoom2D > roomTilemapList = LightTilemapRoom2D.List;
            int roomTilemapCount = roomTilemapList.Count;

            if (roomTilemapCount < 1)
            {
                return;
            }

            for(int i = 0; i < roomTilemapCount; i++)
            {
                LightTilemapRoom2D id = roomTilemapList[i];

                if (id.lightLayer != pass.layerId)
                {
                    continue;
                }

                TilemapRoom.Draw(id, pass.camera);
            }
        }

        private static void DrawLightSprites(Pass pass)
        {
            List<LightSprite2D> spriteRendererList = LightSprite2D.List;
            int spriteRendererCount = spriteRendererList.Count;

            if (spriteRendererCount < 1)
            {
                return;
            }

            LightSprite.Pass.currentTexture = null;

            // Draw Simple
            for(int i = 0; i < spriteRendererCount; i++)
            {
                LightSprite2D id = spriteRendererList[i];

                if (id.lightLayer != pass.layerId)
                {
                    continue;
                }

                LightSprite.Pass.Draw(id, pass.camera);
            }

            if (LightSprite.Pass.currentTexture != null)
            {
                GL.End();
            }
        }

        private static void DrawLightSprites_Scriptable(Pass pass)
        {
            List<Scriptable.LightSprite2D> spriteRendererList = Scriptable.LightSprite2D.List;
            int spriteRendererCount = spriteRendererList.Count;

            if (spriteRendererCount < 1)
            {
                return;
            }

            for(int i = 0; i < spriteRendererCount; i++)
            {
                Scriptable.LightSprite2D id = spriteRendererList[i];

                if (id.LightLayer != pass.layerId)
                {
                    continue;
                }

                LightSprite.Script.DrawScriptable(id, pass.camera);
            }
        }

        private static void DrawLightTextures(Pass pass)
        {
            List<LightTexture2D> lightTextureList = LightTexture2D.List;
            int lightTextureCount = lightTextureList.Count;

            if (lightTextureCount < 1) {
                return;
            }

			for(int i = 0; i < lightTextureCount; i++)
            {
				LightTexture2D id = lightTextureList[i];

				if (id.lightLayer != pass.layerId)
                {
					continue;
				}

				TextureRenderer.Draw(id, pass.camera);
			}
        }

        private static void DrawLightParticleSystem(Pass pass)
        {
            List<LightParticleSystem2D> particleRendererList = LightParticleSystem2D.List;
            int lightParticleSystemCount = particleRendererList.Count;

            if (lightParticleSystemCount < 1)
            {
                return;
            }

			for(int i = 0; i < lightParticleSystemCount; i++)
            {
				LightParticleSystem2D id = particleRendererList[i];

				if (id.lightLayer != pass.layerId)
                {
					continue;
				}

				ParticleRenderer.Draw(id, pass.camera);
			}
        }

        private static void DrawLight(Pass pass)
        {
            List<Light2D> lightList = Light2D.List;

            int lightCount = lightList.Count;

            if (lightCount < 1)
            {
                return;
            }

            for(int i = 0; i < lightCount; i++)
            {
                Light2D id = lightList[i];

                if (id.lightLayer >= 0 && id.lightLayer == pass.layerId)
                {
                    LightSource.Draw(id, pass.camera);
                }

                if (id.occlusionLayer > 0 && (id.occlusionLayer - 1) == pass.layerId)
                {
                    LightSource.DrawOcclusion(id, pass.camera);
                }

                if (id.translucentLayer > 1 && (id.translucentLayer - 2) == pass.layerId)
                {
                    LightSource.DrawTranslucent(id, pass.camera);
                }
            }
        }
    }
}
