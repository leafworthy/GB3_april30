﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Event_Handling;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Event_Handling
{
    [ExecuteInEditMode]
    public class LightEventListenerCount : MonoBehaviour
    {
        private LightCollider2D lightCollider;

        [SerializeField]
        public List<Light2D> lights = new List<Light2D>();

        private void OnEnable()
        {
            lights.Clear();
            
            lightCollider = GetComponent<LightCollider2D>();

            lightCollider?.AddEvent(CollisionEvent);
        }

        private void OnDisable()
        {
            lightCollider?.RemoveEvent(CollisionEvent);
        }

        private void CollisionEvent(LightCollision2D collision)
        {
            switch(collision.state)
            {
                case LightCollision2D.State.OnCollisionEnter:
                    lights.Add(collision.light);
                break;

                case LightCollision2D.State.OnCollisionExit:
                    lights.Remove(collision.light);
                break;
            }
        }
    }
}
