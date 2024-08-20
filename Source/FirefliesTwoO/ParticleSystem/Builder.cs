using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class Builder
    {
        public static ParticleSystem CreateFireflyParticleSystem(int mapID)
        {
            GameObject fireflies = new($"firefly_system_{Mathf.Abs(mapID)}");
            ParticleSystem particleSys = fireflies.GetComponent<ParticleSystem>() ?? fireflies.AddComponent<ParticleSystem>();
            ParticleSystemRenderer renderer = fireflies.GetComponent<ParticleSystemRenderer>() ?? fireflies.AddComponent<ParticleSystemRenderer>();

            ConfigureParticleSystem(particleSys, FFDefOf.FF_Config.particleLifetime);
            ConfigureShapeModule(particleSys);
            ConfigureEmissionModule(particleSys);
            ConfigureNoiseModule(particleSys);
            ConfigureVelocityOverLifetimeModule(particleSys);
            ConfigureSizeOverLifetimeModule(particleSys, FFDefOf.FF_Config.particleSizeFactor);
            ConfigureColorOverLifetimeModule(particleSys);
            Material material = new(GetParticleShader());
            //ConfigureTrailModule(particleSys, material);
            ConfigureRenderer(particleSys, material, Textures.Firefly);

            return particleSys;
        }

        private static void ConfigureParticleSystem(ParticleSystem particleSys, float particleLifetime)
        {
            ParticleSystem.MainModule mainModule = particleSys.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            mainModule.loop = true;
            mainModule.startLifetime = particleLifetime;
            mainModule.startSize = 1f;
            mainModule.startSpeed = 1f;
            mainModule.duration = float.PositiveInfinity;
        }

        private static void ConfigureShapeModule(ParticleSystem particleSys)
        {
            ParticleSystem.ShapeModule shapeModule = particleSys.shape;
            shapeModule.enabled = true;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Vertex;
            shapeModule.randomDirectionAmount = FFDefOf.FF_Config.shapeRandomDirectionAmount.RandomInRange;
        }

        private static void ConfigureEmissionModule(ParticleSystem particleSys)
        {
            ParticleSystem.EmissionModule emissionModule = particleSys.emission;
            emissionModule.enabled = true;
        }
        
        private static void ConfigureNoiseModule(ParticleSystem particleSys)
        {
            ParticleSystem.NoiseModule noiseModule = particleSys.noise;
            noiseModule.enabled = true;
            noiseModule.quality = ParticleSystemNoiseQuality.Medium;
            noiseModule.octaveCount = FFDefOf.FF_Config.noiseOctaveCount;
            noiseModule.frequency = FFDefOf.FF_Config.noiseFrequency;
            noiseModule.positionAmount = FFDefOf.FF_Config.noisePositionAmount;
            noiseModule.strength = FFDefOf.FF_Config.noiseStrength;
            noiseModule.damping = true;
        }

        private static void ConfigureVelocityOverLifetimeModule(ParticleSystem particleSys)
        {
            ParticleSystem.VelocityOverLifetimeModule velocityModule = particleSys.velocityOverLifetime;
            velocityModule.enabled = true;
            
            AnimationCurve curve = new ();
            curve.AddKey(0.0f, 1f);
            curve.AddKey(1.0f, 1f);
            
            velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(0.1f, curve);
        }

        private static void ConfigureSizeOverLifetimeModule(ParticleSystem particleSys, float particleSizeFactor)
        {
            ParticleSystem.SizeOverLifetimeModule sizeModule = particleSys.sizeOverLifetime;
            sizeModule.enabled = true;
            
            AnimationCurve curve = new ();
            curve.AddKey(0f, 1f * particleSizeFactor);
            curve.AddKey(1f, 1f * particleSizeFactor);

            sizeModule.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }

        private static void ConfigureColorOverLifetimeModule(ParticleSystem particleSys)
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSys.colorOverLifetime;
            colorOverLifetimeModule.enabled = true;
            Color color = particleSys.main.startColor.color;
            
            Gradient gradient = new ();
            gradient.SetKeys(
                [
                    new GradientColorKey(color, 0f),
                    new GradientColorKey(color, 1f)
                ],
                [
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0f, 0.4f),
                    new GradientAlphaKey(1f, 0.45f),
                    new GradientAlphaKey(1f, 0.55f),
                    new GradientAlphaKey(0f, 0.6f),
                    new GradientAlphaKey(0f, 1f)
                ]
            );
            colorOverLifetimeModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        private static void ConfigureRenderer(ParticleSystem particleSys, Material material, Texture2D fireflyTexture)
        {
            ParticleSystemRenderer renderer = particleSys.GetComponent<ParticleSystemRenderer>();
            renderer.material = material;
            material.SetTexture(Shader.PropertyToID("_MainTex"), fireflyTexture);
            particleSys.Stop();
        }
        
        // FOR DEBUGGING
        private static void ConfigureTrailModule(ParticleSystem particleSys, Material material)
        {
            ParticleSystem.TrailModule trailModule = particleSys.trails;
            trailModule.enabled = true;
            trailModule.mode = ParticleSystemTrailMode.PerParticle;
            trailModule.dieWithParticles = true;
            ParticleSystemRenderer pSR = particleSys.GetComponent<ParticleSystemRenderer>();
            pSR.trailMaterial = material;
            pSR.trailMaterial.SetColor(Shader.PropertyToID("_Color"), Color.green);
        }
        
        private static Shader GetParticleShader()
        {
            Shader shader = ShaderDatabase.TransparentPostLight;
            
            if (shader != null) return shader;
            FFLog.Warning("Shader 'Custom/TransparentPostLight' not found. Using `Transparent` shader.");
            shader = ShaderDatabase.Transparent;
            
            return shader;
        }
    }
}