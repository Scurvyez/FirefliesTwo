using RimWorld;

namespace FirefliesTwoO
{
    [DefOf]
    public class FFDefOf
    {
        public static FFConfigDef FF_Config;
        public static ThoughtDef FF_SawFireflies;
        public static ThoughtDef FF_CaughtAFirefly;
        public static ThoughtDef FF_SquishedAFirefly;
        public static TraitDef Nimble;
        public static TraitDef NightOwl;
        
        static FFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FFDefOf));
        }
    }
}