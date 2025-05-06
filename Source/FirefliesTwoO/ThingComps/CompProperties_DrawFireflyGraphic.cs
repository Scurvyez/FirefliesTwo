using Verse;

namespace FirefliesTwoO
{
    public class CompProperties_DrawFireflyGraphic : CompProperties
    {
        public GraphicData fireFlyGraphic;
        public float maxOffsetFactorX = 1;
        public float maxOffsetFactorZ = 1;
        
        public CompProperties_DrawFireflyGraphic()
        {
            compClass = typeof(CompDrawFireflyGraphic);
        }
    }
}