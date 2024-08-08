using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class FFSystem
    {
        public GameObject Fireflies;
        public ParticleSystem Particles;
        public ParticleSystemRenderer Renderer;
        public Material Material;

        public float Speed
        {
            get => Particles.main.simulationSpeed;
            set
            {
                ParticleSystem.MainModule main = Particles.main;
                main.simulationSpeed = value;
            }
        }

        public FloatRange Size
        {
            get
            {
                ParticleSystem.MainModule mainModule = Particles.main;
                return new FloatRange(mainModule.startSize.constantMin, mainModule.startSize.constantMax);
            }
            set
            {
                ParticleSystem.MainModule mainModule = Particles.main;
                mainModule.startSize = new ParticleSystem.MinMaxCurve(value.min, value.max);
            }
        }
        
        public float Alpha
        {
            get => Material.color.a;
            set
            {
                Color color = Material.color;
                Material.color = new Color(color.r, color.g, color.b, value);
            }
        }
        
        public Color Color
        {
            get => Renderer.material.color;
            set => Renderer.material.color = value;
        }

        public void Destroy()
        {
            if (Fireflies == null) return;
            Object.Destroy(Fireflies);
            Fireflies = null;
            Particles = null;
            Renderer = null;
        }
    }
}