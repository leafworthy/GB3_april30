using UnityEngine;

namespace _SCRIPTS
{
    public class FallToFloor : MonoBehaviour
    {
        public float rotationRate = 135;


        private Vector3 velocity;
        private float minVelocity = .3f;
        private float maxVelocity = .8f;
        private float floorPoint;
        private bool hasLanded;
        private SpriteRenderer sprite;
        private float rotationVeloctiy;

        public void Fire(Vector3 shootAngle, float ForceMultiplier = 1, float distanceToFloor = 5, string layerName = "Default")
        {
            hasLanded = false;
            velocity = new Vector3(Random.Range(minVelocity, maxVelocity)*shootAngle.x, Random.Range(minVelocity, maxVelocity), 0f)*
                       ForceMultiplier;
            rotationRate = velocity.x*rotationRate;
            floorPoint = transform.position.y - distanceToFloor-Random.Range(0,distanceToFloor);
            sprite = GetComponent<SpriteRenderer>();
            sprite.sortingLayerName = layerName;
        }

        void Update()
        {
            if (hasLanded)
                return;
            transform.Rotate(new Vector3(0, 0, rotationRate * Time.deltaTime*10));
            velocity -= GAME.Gravity * Time.deltaTime;
            transform.position += velocity;
            if (transform.position.y
                <= floorPoint)
            {
                hasLanded = true;
                MAKER.Unmake(gameObject, 3f);
            }
        }
    }
}
