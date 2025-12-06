using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
    public struct Bounds2D
    {
	    public float minX;
	    public float minY;
        public float maxX;
        public float maxY;

        public Bounds2D(Bounds bounds)
        {
            Vector2 min = bounds.min;
            Vector2 max = bounds.max;
            minX = min.x;
            minY = min.y;
            maxX = max.x;
            maxY = max.y;
        }

        public bool Intersects(Bounds2D otherBounds)
        {
            if (minX > otherBounds.maxX || otherBounds.minX > maxX)
                return false;

            // If one rectangle is above other
            if (maxY < otherBounds.minY || otherBounds.maxY < minY)
                return false;

            return true;
        }

        public override string ToString()
        {
            return "Min: (" + minX + ", " + minY + ")  Max: (" + maxX + ", " + maxY + ")";
        }
    }
}
