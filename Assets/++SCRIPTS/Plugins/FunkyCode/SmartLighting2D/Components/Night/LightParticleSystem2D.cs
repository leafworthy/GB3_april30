﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night
    {
    [ExecuteInEditMode]
    public class LightParticleSystem2D : MonoBehaviour {
        public enum Type {Particle};

        public int lightLayer = 0;

        public Color color = Color.white;

        public bool useParticleColor = false;

        public float scale = 1;

        public Texture customParticle;

        private ParticleSystem particleSystem2D;
        private ParticleSystemRenderer particleSystemRenderer2D;
        public ParticleSystem.Particle[] particleArray;

        public static List<LightParticleSystem2D> List = new List<LightParticleSystem2D>();

        public void OnEnable() {
            List.Add(this);

            LightingManager2D.Get();
        }

        public void OnDisable() {
            List.Remove(this);
        }

        public ParticleSystem GetParticleSystem() {
            if (particleSystem2D == null) {
                particleSystem2D = GetComponent<ParticleSystem>();
            }

            return(particleSystem2D);
        }

        public ParticleSystemRenderer GetParticleSystemRenderer() {
            if (particleSystemRenderer2D == null) {
                particleSystemRenderer2D = GetComponent<ParticleSystemRenderer>();
            }

            return(particleSystemRenderer2D);
        }
    }
}