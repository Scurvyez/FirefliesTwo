using RimWorld;

namespace FirefliesTwoO
{
    [DefOf]
    public class FFDefOf
    {
        public static FFConfigDef FF_Config;
        public static ThoughtDef FF_SawFireflies;
        
        static FFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FFDefOf));
        }
    }
}