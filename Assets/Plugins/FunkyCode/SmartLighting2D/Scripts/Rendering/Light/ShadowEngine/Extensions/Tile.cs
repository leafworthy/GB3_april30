using System.Collections.Generic;
using FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D;
using FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using FunkyCode.Utilities;

namespace FunkyCode.SmartLighting2D.Scripts.Rendering.Light.ShadowEngine.Extensions
{
    public class Tile
    {
        static public void Draw(Light2D light, LightTile tile, LightTilemapCollider2D tilemap)
        {
            Base tilemapCollider = tilemap.GetCurrentTilemap();

            List<Polygon2> polygons = tile.GetWorldPolygons(tilemapCollider);

            ShadowEngine.Draw(polygons, 0, 0, 0);
        }
    }
}
