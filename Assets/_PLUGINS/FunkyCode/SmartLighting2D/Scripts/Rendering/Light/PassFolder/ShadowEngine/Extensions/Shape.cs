using System.Collections.Generic;
using FunkyCode.Utilities;

namespace Rendering.Light.Shadow {

    public class Shape {

        public static void Draw(Light2D light, LightCollider2D id) {
            if (id.InLight(light) == false) {
                return;
            }

            light.AddCollider(id);

            foreach(LightColliderShape shape in id.shapes) {
                List<Polygon2> polygons = shape.GetPolygonsWorld();
                
                ShadowEngine.Draw(polygons, shape.shadowDistance, shape.shadowTranslucency);
            }
        }
    }
}