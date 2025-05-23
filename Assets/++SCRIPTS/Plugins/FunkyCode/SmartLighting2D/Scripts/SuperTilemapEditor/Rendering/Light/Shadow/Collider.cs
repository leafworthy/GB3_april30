﻿
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;

#if (SUPER_TILEMAP_EDITOR)

    namespace FunkyCode.SuperTilemapEditorSupport.Light.Shadow
    {
        public class Collider
        {
            static public void Draw(Light2D light, LightTilemapCollider2D id)
            {
                Rendering.Light.ShadowEngine.Draw(id.superTilemapEditor.GetWorldColliders(), 0, 0, 0);
            }
        }
    }

#else

    namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.SuperTilemapEditor.Rendering.Light.Shadow
    {
        public class Collider
        {
            static public void Draw(Light2D light, LightTilemapCollider2D id) {}
        }
    }

#endif
