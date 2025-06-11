using Verse;

namespace FirefliesTwoO
{
    public class CompProperties_DrawFireflyGraphic : CompProperties
    {
        public GraphicData fireFlyGraphic;
        public IntRange nextFlickerTick = IntRange.One;
        public IntRange transitionDuration = IntRange.One;
        public IntRange flickerDuration = IntRange.One;
        public float maxOffsetFactorX = 1;
        public float maxOffsetFactorZ = 1;
        public float initialOffsetZ = 0.12f;
        
        public CompProperties_DrawFireflyGraphic()
        {
            compClass = typeof(CompDrawFireflyGraphic);
        }
    }
}