using RimWorld;

namespace FirefliesTwoO
{
    [DefOf]
    public class FFDefOf
    {
        public static FFConfigDef FF_Config;

        static FFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FFDefOf));
        }
    }
}