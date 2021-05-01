using Pathfinding;
using UnityEngine;

namespace _SCRIPTS
{
    public class UpdatePathingOnDestroy : MonoBehaviour
    {
        void Start()
        {
            var dest = GetComponent<DefenceHandler>();
            dest.OnDead += OnDead;
        }

        private void OnDead()
        {
            var myBounds = GetComponent<Collider2D>().bounds;
            var guo = new GraphUpdateObject(myBounds);
            guo.updatePhysics = true;
            AstarPath.active.UpdateGraphs(guo);
        }

    }
}
