using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class ParticleBuilder
    {
        public static void ConfigureParticleSystem(ParticleSystem particleSys, int maxNumFireflies, float particleLifetime)
        {
            ParticleSystem.MainModule mainModule = particleSys.main;
            mainModule.loop = false;
            mainModule.maxParticles = maxNumFireflies;
            mainModule.startLifetime = particleLifetime;
            mainModule.startSize = 1f;
            mainModule.startSpeed = 10f;
            mainModule.duration = 99999;
        }

        public static void ConfigureShapeModule(ParticleSystem particleSys, Vector3 areaSize)
        {
            ParticleSystem.ShapeModule shapeModule = particleSys.shape;
            shapeModule.enabled = true;
            shapeModule.shapeType = ParticleSystemShapeType.Box;
            shapeModule.scale = areaSize;
            shapeModule.randomDirectionAmount = FFDefOf.FF_Config.shapeRandomDirectionAmount.RandomInRange;
        }

        public static void ConfigureEmissionModule(ParticleSystem particleSys, float emissionRate)
        {
            ParticleSystem.EmissionModule emissionModule = particleSys.emission;
            emissionModule.enabled = true;
            emissionModule.rateOverTime = emissionRate;
        }

        public static void ConfigureNoiseModule(ParticleSystem particleSys)
        {
            ParticleSystem.NoiseModule noiseModule = particleSys.noise;
            noiseModule.enabled = true;
            noiseModule.octaveCount = FFDefOf.FF_Config.noiseOctaveCount;
            noiseModule.frequency = FFDefOf.FF_Config.noiseFrequency;
            noiseModule.positionAmount = FFDefOf.FF_Config.noisePositionAmount;
            noiseModule.damping = true;
            noiseModule.strength = FFDefOf.FF_Config.noiseStrength;
            noiseModule.quality = ParticleSystemNoiseQuality.Medium;
        }
        
        // FOR DEBUGGING
        public static void ConfigureTrailModule(ParticleSystem particleSys)
        {
            ParticleSystem.TrailModule trailModule = particleSys.trails;
            trailModule.enabled = true;
            trailModule.mode = ParticleSystemTrailMode.PerParticle;
            trailModule.dieWithParticles = true;
            trailModule.inheritParticleColor = true;
        }

        public static void ConfigureSizeOverLifetimeModule(ParticleSystem particleSys, float particleSizeFactor)
        {
            ParticleSystem.SizeOverLifetimeModule sizeModule = particleSys.sizeOverLifetime;
            sizeModule.enabled = true;
            
            AnimationCurve curve = new ();
            curve.AddKey(0f, 1f * particleSizeFactor);
            curve.AddKey(1f, 1f * particleSizeFactor);

            sizeModule.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }

        public static void ConfigureColorOverLifetimeModule(ParticleSystem particleSys, Color baseColor)
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSys.colorOverLifetime;
            colorOverLifetimeModule.enabled = true;

            Gradient gradient = new ();
            gradient.SetKeys(
                [
                    new GradientColorKey(baseColor, 0f),
                    new GradientColorKey(baseColor, 1f)
                ],
                [
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.25f),
                    new GradientAlphaKey(1f, 0.75f),
                    new GradientAlphaKey(0f, 1f)
                ]
            );
            colorOverLifetimeModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        public static void ConfigureRenderer(ParticleSystem particleSys, Material material, Texture2D fireflyTexture)
        {
            ParticleSystemRenderer renderer = particleSys.GetComponent<ParticleSystemRenderer>();
            renderer.material = material;
            material.SetTexture(Shader.PropertyToID("_MainTex"), fireflyTexture);
            particleSys.Stop();
        }
        
        public static Shader GetParticleShader()
        {
            Shader shader = ShaderDatabase.TransparentPostLight;
            
            if (shader != null) return shader;
            FFLog.Warning("Shader 'Custom/TransparentPostLight' not found. Using `Standard` shader.");
            shader = Shader.Find("Standard");
            
            return shader;
        }
    }
}