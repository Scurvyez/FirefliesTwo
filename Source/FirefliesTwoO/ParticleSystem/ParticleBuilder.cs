using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class ParticleBuilder
    {
        private static readonly Color DefaultFireflyColor = new(1f, 1f, 0f, 1f);
        
        public static ParticleSystem CreateFireflyParticleSystem(Vector3 mapSize, Mesh spawnAreaMesh)
        {
            GameObject fireflies = new("FFSystem");

            ParticleSystem particleSys = fireflies.GetComponent<ParticleSystem>() ?? fireflies.AddComponent<ParticleSystem>();
            ParticleSystemRenderer renderer = fireflies.GetComponent<ParticleSystemRenderer>() ?? fireflies.AddComponent<ParticleSystemRenderer>();

            ConfigureParticleSystem(particleSys, FFDefOf.FF_Config.particlesMaxCount, FFDefOf.FF_Config.particleLifetime);
            ConfigureShapeModule(particleSys, mapSize, spawnAreaMesh);
            ConfigureEmissionModule(particleSys, FFDefOf.FF_Config.particleEmissionRate);
            ConfigureNoiseModule(particleSys);
            ConfigureSizeOverLifetimeModule(particleSys, FFDefOf.FF_Config.particleSizeFactor);
            ConfigureColorOverLifetimeModule(particleSys, Color.green);
            Material material = new(GetParticleShader());
            ConfigureTrailModule(particleSys, material);
            ConfigureRenderer(particleSys, material, Assets.Firefly);

            return particleSys;
        }

        private static void ConfigureParticleSystem(ParticleSystem particleSys, int maxNumFireflies, float particleLifetime)
        {
            ParticleSystem.MainModule mainModule = particleSys.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            mainModule.loop = false;
            mainModule.maxParticles = maxNumFireflies;
            mainModule.startLifetime = particleLifetime;
            mainModule.startSize = 1f;
            mainModule.startSpeed = 1f;
            mainModule.duration = 99999;
        }

        private static void ConfigureShapeModule(ParticleSystem particleSys, Vector3 mapSize, Mesh spawnAreaMesh)
        {
            ParticleSystem.ShapeModule shapeModule = particleSys.shape;
            shapeModule.enabled = true;
            //shapeModule.scale = mapSize;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = spawnAreaMesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Vertex;
            shapeModule.randomDirectionAmount = FFDefOf.FF_Config.shapeRandomDirectionAmount.RandomInRange;
        }

        private static void ConfigureEmissionModule(ParticleSystem particleSys, float emissionRate)
        {
            ParticleSystem.EmissionModule emissionModule = particleSys.emission;
            emissionModule.enabled = true;
            emissionModule.rateOverTime = emissionRate;
        }
        
        private static void ConfigureNoiseModule(ParticleSystem particleSys)
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

        private static void ConfigureSizeOverLifetimeModule(ParticleSystem particleSys, float particleSizeFactor)
        {
            ParticleSystem.SizeOverLifetimeModule sizeModule = particleSys.sizeOverLifetime;
            sizeModule.enabled = true;
            
            AnimationCurve curve = new ();
            curve.AddKey(0f, 1f * particleSizeFactor);
            curve.AddKey(1f, 1f * particleSizeFactor);

            sizeModule.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }

        private static void ConfigureColorOverLifetimeModule(ParticleSystem particleSys, Color baseColor)
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
            pSR.trailMaterial.SetColor(Shader.PropertyToID("_Color"), Color.yellow);
        }
        
        private static Shader GetParticleShader()
        {
            Shader shader = ShaderDatabase.TransparentPostLight;
            
            if (shader != null) return shader;
            FFLog.Warning("Shader 'Custom/TransparentPostLight' not found. Using `Standard` shader.");
            shader = Shader.Find("Standard");
            
            return shader;
        }
    }
}